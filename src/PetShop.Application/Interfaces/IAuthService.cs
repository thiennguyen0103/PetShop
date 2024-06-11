using PetShop.Application.Common.Models.Auth;
using PetShop.Application.Wrappers;

namespace PetShop.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Response<AuthResponse>> LoginAsync(AuthRequest request, string ipAddress);
        Task<Response<Guid>> RegisterAsync(RegisterRequest request, string origin);
        Task<Response<Guid>> ConfirmEmailAsync(Guid userId, string code);
        Task ForgotPassword(ForgotPasswordRequest model, string origin);
        Task<Response<Guid>> VerifyEmail(VerifyEmailRequest model);
    }
}
