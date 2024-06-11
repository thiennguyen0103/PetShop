namespace PetShop.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = new Category();
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = new Supplier();
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}
