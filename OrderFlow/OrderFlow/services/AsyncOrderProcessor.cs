using System.Diagnostics;
using OrderFlow.Models;

namespace OrderFlow.Services;

public class AsyncOrderProcessor
{
    public async Task ProcessOrderAsync(Order order)
    {
        var service = new ExternalServiceSimulator();
        var sw = Stopwatch.StartNew();

        var t1 = service.CheckInventoryAsync(order.Items.First().Product);
        var t2 = service.ValidatePaymentAsync(order);
        var t3 = service.CalculateShippingAsync(order);

        await Task.WhenAll(t1, t2, t3);

        Console.WriteLine($"Order {order.Id} done in {sw.ElapsedMilliseconds} ms");
    }

    public async Task ProcessMultipleOrdersAsync(List<Order> orders)
    {
        var semaphore = new SemaphoreSlim(3);
        int done = 0;

        var tasks = orders.Select(async order =>
        {
            await semaphore.WaitAsync();
            try
            {
                await ProcessOrderAsync(order);
                Interlocked.Increment(ref done);
                Console.WriteLine($"Processed {done}/{orders.Count}");
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }
}