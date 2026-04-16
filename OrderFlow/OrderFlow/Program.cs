using System.Text.Json;
using OrderFlow.Services;
using OrderFlow.Data;
using OrderFlow.Models;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("\nZADANIE 1 (Lab3): JSON/XML");

        var repo = new OrderFlow.Persistence.OrderRepository();

        var jsonPath = Path.Combine("data", "orders.json");
        var xmlPath = Path.Combine("data", "orders.xml");

        await repo.SaveToJsonAsync(SampleData.Orders, jsonPath);
        await repo.SaveToXmlAsync(SampleData.Orders, xmlPath);

        var empty = new List<Order>();

        var fromJson = await repo.LoadFromJsonAsync(jsonPath);
        var fromXml = await repo.LoadFromXmlAsync(xmlPath);

        Console.WriteLine($"JSON count: {fromJson.Count}");
        Console.WriteLine($"XML count: {fromXml.Count}");

        Console.WriteLine($"JSON total: {fromJson.Sum(o => o.TotalAmount)}");
        Console.WriteLine($"XML total: {fromXml.Sum(o => o.TotalAmount)}");
        
        
        Console.WriteLine("\nZADANIE 2 (XML REPORT):");

        var builder = new XmlReportBuilder();

        var report = builder.BuildReport(SampleData.Orders);

        await builder.SaveReportAsync(report, "data/report.xml");

        Console.WriteLine("Report saved to data/report.xml");

        var expensive = await builder.FindHighValueOrderIdsAsync("data/report.xml", 1000m);

        Console.WriteLine("Orders > 1000:");
        foreach (var id in expensive)
        {
            Console.WriteLine($"  Order {id}");
        }
        
        
        Console.WriteLine("\nZADANIE 3 (WATCHER):");

        var processor = new OrderProcessor();

        processor.StatusChanged += (s, e) =>
            Console.WriteLine($"STATUS: {e.OldStatus} -> {e.NewStatus}");

        using var watcher = new InboxWatcher("inbox", processor);

        _ = Task.Run(async () =>
        {
            for (int i = 1; i <= 5; i++)
            {
                await Task.Delay(3000);

                var path = $"inbox/orders_{i}.json";
                await repo.SaveToJsonAsync(SampleData.Orders.Take(2), path);

                Console.WriteLine($"[TEST] file created: {path}");
            }
        });
        Console.ReadLine();
        
    }
    
    
    
}