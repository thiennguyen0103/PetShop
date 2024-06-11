using System.ComponentModel.DataAnnotations;

namespace PetShop.Application.Common.Models.Auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
