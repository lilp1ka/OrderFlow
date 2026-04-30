namespace OrderFlow.Models;

public class OrderStatusChangedEventArgs : EventArgs
{
    public Order Order { get; set; } = null!;
    public OrderStatus OldStatus { get; set; }
    public OrderStatus NewStatus { get; set; }
    public DateTime Timestamp { get; set; }
}

public class OrderValidationEventArgs : EventArgs
{
    public Order Order { get; set; } = null!;
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}