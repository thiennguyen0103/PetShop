namespace PetShop.Application.Common.Models.Auth
{
    public class AuthResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public bool IsVerified { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
