using OrderFlow.Data;
using OrderFlow.Services;
using OrderFlow.Models;

class Program
{
    static void Main()
    {
        var orders = SampleData.Orders;

        Console.WriteLine("order validation: ");
        var order1 = orders[0];
        var order2 = orders[4]; 
        if(OrderValidator.ValidateAll(order1, out var errors1))
            Console.WriteLine($"Order {order1.Id} is valid");
        else Console.WriteLine($"Order {order1.Id} invalid: {string.Join(", ", errors1)}");

        if(OrderValidator.ValidateAll(order2, out var errors2))
            Console.WriteLine($"Order {order2.Id} is valid");
        else Console.WriteLine($"Order {order2.Id} invalid: {string.Join(", ", errors2)}");

        Console.WriteLine("\n processing order: ");
        var vipOrders = OrderProcessor.FilterOrders(orders, o=>o.Customer.IsVip);
        OrderProcessor.PrintOrders(vipOrders);

        var totals = OrderProcessor.ProjectOrders(orders, o=>new { o.Id, o.TotalAmount });
        foreach(var t in totals) Console.WriteLine($"Order {t.Id} total: {t.TotalAmount}");

        var sum = OrderProcessor.AggregateOrders(orders, os=>os.Sum(o=>o.TotalAmount));
        Console.WriteLine($"total sum of all orders: {sum}");
    }
}