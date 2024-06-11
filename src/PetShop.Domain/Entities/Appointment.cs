using System.ComponentModel.DataAnnotations;

namespace PetShop.Domain.Entities;

public class Appointment : BaseEntity
{
    public string ServiceType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; } = DateTime.UtcNow;
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = new Customer();
    public Guid PetId { get; set; }
    public Pet Pet { get; set; } = new Pet();
}
