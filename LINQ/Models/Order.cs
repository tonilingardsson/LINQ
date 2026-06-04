namespace LINQ.Models
{
    public class Order
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }
    public double TotalAmount { get; set; }
    public string Status { get; set; } = null!;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public List<OrderDetail> OrderDetails { get; set; } = new();
}
}