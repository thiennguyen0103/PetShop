using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using PetShop.Application.Common.Models.Auth;
using PetShop.Application.Enums;
using PetShop.Application.Exceptions;
using PetShop.Application.Interfaces;
using PetShop.Application.Models.Email;
using PetShop.Application.Wrappers;
using PetShop.Domain.Entities;
using PetShop.Domain.Settings;
using PetShop.Infrastructure.Identity.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PetShop.Infrastructure.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;
        private readonly JWTSettings _jwtSettings;

        public AuthService(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            SignInManager<User> signInManager,
            IEmailService emailService,
            JWTSettings jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _jwtSettings = jwtSettings;
        }

        private async Task<string> SendVerificationEmail(User user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/auth/confirm-email/";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString());
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //Email Service Call Here
            return verificationUri;
        }

        public async Task<Response<Guid>> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName is not null)
            {
                throw new ApiException($"Username '{request.UserName}' is already taken.");
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail is not null)
            {
                throw new ApiException($"Email '{request.Email}' is already registered");
            }

            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Client.ToString());
                var verificationUri = await SendVerificationEmail(user, origin);

                //TODO: Attach Email Service here and configure it via appsettings
                await _emailService.SendAsync(new EmailRequest()
                {
                    From = "thiennguyen.fsd@gmail.com",
                    To = user.Email,
                    Body = $"Please confirm your account by visiting this URL {verificationUri}",
                    Subject = "Confirm Registration"
                });
                return new Response<Guid>(
                    user.Id,
                    message: $"User registered. Please confirm your account by visiting this URL {verificationUri}"
                );
            }
            else
            {
                throw new ApiException($"{result.Errors}");
            }
        }

        public async Task<Response<Guid>> ConfirmEmailAsync(Guid userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return new Response<Guid>(user.Id, message: $"Account confirmed for {user.Email}. You can now login your account.");
            }
            else
            {
                throw new ApiException($"An error occurred while confirming {user.Email}");
            }
        }

        public Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<AuthResponse>> LoginAsync(AuthRequest request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                throw new ApiException($"No accounts registered with {request.Email}");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new ApiException($"Invalid credential for '{user.UserName}'");
            }
            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var refreshToken = GenerateRefreshToken(ipAddress);
            AuthResponse response = new()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Roles = rolesList.ToList(),
                IsVerified = user.EmailConfirmed,
                RefreshToken = refreshToken.Token
            };
            return new Response<AuthResponse>(response, $"Authenticated {user.UserName}");
        }

        private async Task<JwtSecurityToken> GenerateJWToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString()),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private string RandomTokenString()
        {
            var randomNumber = new Byte[32];
            RandomNumberGenerator.Fill(randomNumber);
            string token = Convert.ToBase64String(randomNumber);
            return token;
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}
