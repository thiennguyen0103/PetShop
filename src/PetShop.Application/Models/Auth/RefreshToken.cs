namespace PetShop.Application.Common.Models.Auth
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow > Expires;
        public DateTime CreatedAt { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplaceByTokenn { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
