using OrderFlow.Models;

namespace OrderFlow.Services;

public class OrderProcessor
{
    
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<OrderValidationEventArgs>? ValidationCompleted;
   
    public void ProcessOrder(Order order)
    {
        var errors = new List<string>();

        if (order.Items == null || !order.Items.Any())
            errors.Add("No items");

        bool isValid = !errors.Any();

        ValidationCompleted?.Invoke(this, new OrderValidationEventArgs
        {
            Order = order,
            IsValid = isValid,
            Errors = errors
        });

        if (!isValid) return;

        ChangeStatus(order, OrderStatus.Validated);
        ChangeStatus(order, OrderStatus.Processing);
        ChangeStatus(order, OrderStatus.Completed);
    }

    private void ChangeStatus(Order order, OrderStatus newStatus)
    {
        var old = order.Status;
        order.Status = newStatus;

        StatusChanged?.Invoke(this, new OrderStatusChangedEventArgs
        {
            Order = order,
            OldStatus = old,
            NewStatus = newStatus,
            Timestamp = DateTime.Now
        });
    }
    
    
    public static void PrintOrders(List<Order> orders)
    {
        orders.ForEach(o => Console.WriteLine($"Order {o.Id}, Customer {o.Customer.Name}, Total {o.TotalAmount}"));
    }

    public static List<Order> FilterOrders(List<Order> orders, Predicate<Order> predicate)
    {
        return orders.FindAll(predicate);
    }

    public static List<T> ProjectOrders<T>(List<Order> orders, Func<Order,T> selector)
    {
        return orders.Select(selector).ToList();
    }

    public static decimal AggregateOrders(List<Order> orders, Func<IEnumerable<Order>,decimal> aggregator)
    {
        return aggregator(orders);
    }
}