namespace PetShop.Domain.Entities;

public class OrderDetail : BaseEntity
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = new Order();
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = new Product();
}
