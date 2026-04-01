using OrderFlow.Models;

namespace OrderFlow.Services;

public static class OrderProcessor
{
    public static void PrintOrders(List<Order> orders)
    {
        orders.ForEach(o => Console.WriteLine($"Order {o.Id}, Customer {o.Customer.Name}, Total {o.TotalAmount}"));
    }

    public static List<Order> FilterOrders(List<Order> orders, Predicate<Order> predicate)
    {
        return orders.FindAll(predicate);
    }

    public static List<T> ProjectOrders<T>(List<Order> orders, Func<Order,T> selector)
    {
        return orders.Select(selector).ToList();
    }

    public static decimal AggregateOrders(List<Order> orders, Func<IEnumerable<Order>,decimal> aggregator)
    {
        return aggregator(orders);
    }
}