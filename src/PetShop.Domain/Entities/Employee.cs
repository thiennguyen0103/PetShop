namespace PetShop.Domain.Entities;

public class Employee : BaseEntity
{
    public string Position { get; set; } = string.Empty;
    public DateTime DateHired { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public User User { get; set; } = new User();
}
