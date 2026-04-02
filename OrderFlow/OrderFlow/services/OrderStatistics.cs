using System.Collections.Concurrent;
using OrderFlow.Models;

namespace OrderFlow.Services;
public class OrderStatistics
{
    // Счетчики и данные
    public int TotalProcessed;
    public decimal TotalRevenue;
    public ConcurrentDictionary<OrderStatus, int> OrdersPerStatus = new();
    public List<string> ProcessingErrors = new();

    private object _lock = new();

    // ---------------------------------------
    // Потокобезопасное добавление заказа
    // ---------------------------------------
    public void AddOrderSafe(Order order)
    {
        // Количество обработанных
        Interlocked.Increment(ref TotalProcessed);

        // Доход и ошибки — через lock
        lock (_lock)
        {
            TotalRevenue += order.TotalAmount;
            if (!order.IsValid)
                ProcessingErrors.Add($"Order {order.Id} invalid: {string.Join(", ", order.ValidationErrors)}");
        }

        // Статус заказа — через ConcurrentDictionary
        OrdersPerStatus.AddOrUpdate(order.Status, 1, (_, old) => old + 1);
    }

    // ---------------------------------------
    // Небезопасное добавление (для демонстрации бага)
    // ---------------------------------------
    public void AddOrderUnsafe(Order order)
    {
        // просто без блокировок
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