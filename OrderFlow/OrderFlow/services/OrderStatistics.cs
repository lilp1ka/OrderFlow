using System.Collections.Concurrent;
using OrderFlow.Models;

namespace OrderFlow.Services;
public class OrderStatistics
{
    public int TotalProcessed;
    public decimal TotalRevenue;
    public ConcurrentDictionary<OrderStatus, int> OrdersPerStatus = new();
    public List<string> ProcessingErrors = new();

    private object _lock = new();

    public void AddOrderSafe(Order order)
    {
        Interlocked.Increment(ref TotalProcessed);

        lock (_lock)
        {
            TotalRevenue += order.TotalAmount;
            if (!order.IsValid)
                ProcessingErrors.Add($"Order {order.Id} invalid: {string.Join(", ", order.ValidationErrors)}");
        }

        OrdersPerStatus.AddOrUpdate(order.Status, 1, (_, old) => old + 1);
    }
    public void AddOrderUnsafe(Order order)
    {
        TotalProcessed++;
        TotalRevenue += order.TotalAmount;

        if (!order.IsValid)
            ProcessingErrors.Add($"Order {order.Id} invalid: {string.Join(", ", order.ValidationErrors)}");

        if (OrdersPerStatus.ContainsKey(order.Status))
            OrdersPerStatus[order.Status]++;
        else
            OrdersPerStatus[order.Status] = 1;
    }
}