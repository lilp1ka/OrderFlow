using System;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Models;
using OrderFlow.Services;
using System.Collections.Generic;
using OrderFlow.Data;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("zadanie 1: ");

        var processor = new OrderProcessor();

        processor.StatusChanged += (s, e) =>
            Console.WriteLine($"STATUS: {e.OldStatus} -> {e.NewStatus}");
        processor.StatusChanged += (s, e) =>
            Console.WriteLine($"EMAIL: Order {e.Order.Id} changed status");
        processor.ValidationCompleted += (s, e) =>
            Console.WriteLine($"VALIDATION: Order {e.Order.Id} valid: {e.IsValid}");

        foreach (var order in SampleData.Orders.Take(2))
        {
            processor.ProcessOrder(order);
        }

        Console.WriteLine("\nZADANIE 2: ");

        var asyncProcessor = new AsyncOrderProcessor();

        await asyncProcessor.ProcessMultipleOrdersAsync(SampleData.Orders);

        Console.WriteLine("\nZADANIE 3: ");

        var stats = new OrderStatistics();

        Parallel.ForEach(SampleData.Orders, order =>
        {
            stats.AddOrderSafe(order);
        });

        Console.WriteLine($"\nTotal processed: {stats.TotalProcessed}");
        Console.WriteLine($"Total revenue: {stats.TotalRevenue}");

        Console.WriteLine("Orders per status:");
        foreach (var kvp in stats.OrdersPerStatus)
        {
            Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
        }

        Console.WriteLine("Processing errors:");
        foreach (var err in stats.ProcessingErrors)
        {
            Console.WriteLine($"  {err}");
        }
    }
}