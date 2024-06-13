using Microsoft.AspNetCore.Mvc;
using PetShop.Application.Interfaces;
using PetShop.Application.Common.Models.Auth;

namespace PetShop.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginSync(AuthRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _authService.LoginAsync(request, origin));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterSync(RegisterRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _authService.RegisterAsync(request, origin));
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] Guid userId, [FromQuery] string code)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _authService.ConfirmEmailAsync(userId, code));

        }
    }
}
