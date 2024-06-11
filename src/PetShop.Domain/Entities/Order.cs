namespace PetShop.Domain.Entities;

public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = new Customer();
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
