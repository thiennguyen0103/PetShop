using PetShop.Application.Models.Email;

namespace PetShop.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}
