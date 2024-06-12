namespace BackEnd_Project.Models;

public class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string OrderStatus { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
    public string ShippingMethod { get; set; }
    public string PaymentMethod { get; set; }
    public string ShippingAddress { get; set; }
}