using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Models;

namespace OrderFlow.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(OrderFlowContext db)
    {
        if (await db.Customers.AnyAsync())
            return;

        db.Products.AddRange(SampleData.Products);
        db.Customers.AddRange(SampleData.Customers);

        await db.SaveChangesAsync();

        // canceled ordr for delete test
        if (SampleData.Orders.Count > 0)
            SampleData.Orders[0].Status = OrderStatus.Cancelled;

        db.Orders.AddRange(SampleData.Orders);

        await db.SaveChangesAsync();

        Console.WriteLine("Database seeded.");
    }
}