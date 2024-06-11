namespace PetShop.Domain.Entities;

public class PetDetail : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public string HealthStatus { get; set; } = string.Empty;
    public string VaccinationStatus { get; set; } = string.Empty;
    public Guid PetId { get; set; }
    public Pet Pet { get; set; } = new Pet();
}
