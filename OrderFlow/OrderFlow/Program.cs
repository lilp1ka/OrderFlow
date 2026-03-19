using OrderFlow.Data;

var orders = SampleData.Orders;

foreach (var order in orders)
{
    Console.WriteLine($"Order {order.Id}, Total: {order.TotalAmount}");
}
