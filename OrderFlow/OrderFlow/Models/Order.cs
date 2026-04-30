namespace OrderFlow.Models;

public class Order
{
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public OrderStatus Status { get; set; }

    public string? Notes { get; set; }

    public bool IsValid { get; set; } = true;

    public List<string> ValidationErrors { get; set; } = new();

    public List<OrderItem> Items { get; set; } = new();

    public decimal TotalAmount => Items.Sum(x => x.TotalPrice);
}