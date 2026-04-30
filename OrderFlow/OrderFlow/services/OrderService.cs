using Microsoft.EntityFrameworkCore;
using OrderFlow.Models;
using OrderFlow.Persistence;

namespace OrderFlow.Services;

public class OrderService
{
    public async Task ProcessOrderAsync(OrderFlowContext db, int orderId)
    {
        using var tx = await db.Database.BeginTransactionAsync();

        try
        {
            var order = await db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstAsync(o => o.Id == orderId);

            if (order.Status != OrderStatus.New)
                throw new Exception("Only NEW orders can be processed");

            // check stock
            foreach (var item in order.Items)
            {
                if (item.Product.Stock < item.Quantity)
                    throw new Exception($"Not enough stock for {item.Product.Name}");
            }

            // decrease stock
            foreach (var item in order.Items)
            {
                item.Product.Stock -= item.Quantity;
            }

            order.Status = OrderStatus.Completed;

            await db.SaveChangesAsync();
            await tx.CommitAsync();

            Console.WriteLine($"Order {orderId} processed SUCCESS");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine($"FAILED: {ex.Message}");
        }
    }
}