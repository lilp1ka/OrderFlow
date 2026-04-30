using Microsoft.EntityFrameworkCore;
using OrderFlow.Models;
using OrderFlow.Persistence;
using OrderFlow.Services;

class Program
{
    static async Task Main()
    {
        using var db = new OrderFlowContext();

        await db.Database.MigrateAsync();
        await DatabaseSeeder.SeedAsync(db);

        Console.WriteLine("CREATE:");
        if (!db.Orders.Any(o => o.Notes == "CreatedByCrud"))
        {

            var customer = db.Customers
                .OrderBy(c => c.Id)
                .First();

            var products = db.Products
                .OrderBy(p => p.Id)
                .Take(2)
                .ToList();
            var newOrder = new Order
            {
                CustomerId = customer.Id,
                Status = OrderStatus.New,
                Notes = "Fresh order",
                Date = DateTime.Now,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = products[0].Id,
                        Quantity = 2,
                        UnitPrice = products[0].Price
                    },
                    new OrderItem
                    {
                        ProductId = products[1].Id,
                        Quantity = 1,
                        UnitPrice = products[1].Price
                    }
                }
            };

            db.Orders.Add(newOrder);
            await db.SaveChangesAsync();

            Console.WriteLine($"Added order #{newOrder.Id}");
        }


        Console.WriteLine("\nREAD:");

        var orders = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .ToListAsync();

        foreach (var order in orders)
        {
            Console.WriteLine($"Order #{order.Id} | {order.Customer.Name} | {order.Status}");

            foreach (var item in order.Items)
            {
                Console.WriteLine($"   {item.Product.Name} x{item.Quantity}");
            }
        }



        Console.WriteLine("\nUPDATE:");

        var updateOrder = await db.Orders
            .FirstOrDefaultAsync(o => o.Status == OrderStatus.New);

        if (updateOrder != null)
        {
            updateOrder.Status = OrderStatus.Processing;
            updateOrder.Notes = "Updated in CRUD";

            await db.SaveChangesAsync();

            Console.WriteLine($"Updated order #{updateOrder.Id}");
        }



        Console.WriteLine("\nDELETE:");

        var cancelOrder = await db.Orders
            .FirstOrDefaultAsync(o => o.Status == OrderStatus.Cancelled);

        if (cancelOrder != null)
        {
            db.Orders.Remove(cancelOrder);
            await db.SaveChangesAsync();

            Console.WriteLine($"Deleted cancelled order #{cancelOrder.Id}");
        }
        else
        {
            Console.WriteLine("No cancelled orders found.");
        }
        
        
        Console.WriteLine("\nVIP ORDERS:");

        var vipOrders = db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .AsEnumerable()
            .Where(o => o.Customer.IsVip)
            .Select(o => new
            {
                o.Id,
                Customer = o.Customer.Name,
                Total = o.Items.Sum(i => i.Quantity * i.UnitPrice)
            })
            .Where(x => x.Total > 100)
            .ToList();

        foreach (var o in vipOrders)
            Console.WriteLine($"Order {o.Id} | {o.Customer} | {o.Total}");
        
        
        
        Console.WriteLine("CUSTOMER RANKING:");

        var ranking = db.Customers
            .Include(c => c.Orders)
            .ThenInclude(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsEnumerable()
            .Select(c => new
            {
                c.Name,
                Total = c.Orders
                    .SelectMany(o => o.Items)
                    .Sum(i => i.Quantity * i.UnitPrice)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        foreach (var r in ranking)
        {
            Console.WriteLine($"{r.Name} | {r.Total}");
        }
        
        
        
        Console.WriteLine("\nAVG ORDER VALUE BY CITY: ");

        var avgByCity = db.Orders
            .GroupBy(o => o.Customer.City)
            .Select(g => new
            {
                City = g.Key,
                Avg = g.Average(o => o.Items.Sum(i => i.Quantity * i.UnitPrice))
            })
            .ToList();

        foreach (var c in avgByCity)
            Console.WriteLine($"{c.City} => {c.Avg}");
        
        
        
        Console.WriteLine("\nUNUSED PRODUCTS: ");

        var unused = db.Products
            .Where(p => !p.OrderItems.Any())
            .ToList();

        foreach (var p in unused)
            Console.WriteLine(p.Name);
        
        
        
        Console.WriteLine("\nDYNAMIC QUERY: ");

        OrderStatus? statusFilter = OrderStatus.New;
        decimal? minTotal = 50;

        var query = db.Orders.AsQueryable();

        if (statusFilter != null)
            query = query.Where(o => o.Status == statusFilter);

        if (minTotal != null)
        {
            query = query.Where(o =>
                o.Items.Sum(i => i.Quantity * i.UnitPrice) >= minTotal);
        }

        var result = query.ToList();

        foreach (var o in result)
            Console.WriteLine($"Order {o.Id} | {o.Status}");
        
        
        
        var service = new OrderService();

        await service.ProcessOrderAsync(db, 1);
        
        // FAIL (celowo maly stock)
        await service.ProcessOrderAsync(db, 2);
    }
    
    
}