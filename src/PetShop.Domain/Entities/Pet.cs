namespace PetShop.Domain.Entities;

public class Pet : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public PetDetail PetDetail { get; set; } = new PetDetail();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
