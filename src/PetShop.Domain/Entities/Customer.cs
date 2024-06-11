namespace PetShop.Domain.Entities;

public class Customer : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = new User();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
