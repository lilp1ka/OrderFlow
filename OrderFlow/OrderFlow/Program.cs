using System;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Data;
using OrderFlow.Models;
using OrderFlow.Services;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("=== ЗАДАНИЕ 1: События ===");
        var processor = new OrderProcessor();

        // Подписчики событий
        processor.StatusChanged += (s, e) =>
            Console.WriteLine($"STATUS: {e.OldStatus} -> {e.NewStatus}");
        processor.ValidationCompleted += (s, e) =>
            Console.WriteLine($"VALIDATION: {(e.IsValid ? "valid" : "invalid")}");
        processor.StatusChanged += (s, e) =>
            Console.WriteLine($"EMAIL: Order {e.Order.Id} changed status");

        // Пример обработки первых 2 заказов
        foreach (var order in SampleData.Orders.Take(2))
            processor.ProcessOrder(order);

        Console.WriteLine("\n=== ЗАДАНИЕ 2: Async обработка ===");
        var asyncProcessor = new AsyncOrderProcessor();

        // Обработка нескольких заказов параллельно
        await asyncProcessor.ProcessMultipleOrdersAsync(SampleData.Orders);

        Console.WriteLine("\n=== ЗАДАНИЕ 3: Thread-safe статистика ===");
        var stats = new OrderStatistics();

        Parallel.ForEach(SampleData.Orders, order =>
        {
            stats.AddOrderSafe(order); // потокобезопасное добавление
        });

        Console.WriteLine($"\nTotal processed: {stats.TotalProcessed}");
        Console.WriteLine($"Total revenue: {stats.TotalRevenue}");
        Console.WriteLine("Orders per status:");
        foreach (var kvp in stats.OrdersPerStatus)
            Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
        Console.WriteLine("Processing errors:");
        foreach (var err in stats.ProcessingErrors)
            Console.WriteLine($"  {err}");
    }
}