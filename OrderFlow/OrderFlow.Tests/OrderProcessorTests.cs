using OrderFlow.Models;
using OrderFlow.Services;

namespace OrderFlow.Tests;

public class OrderProcessorTests
{
    [Fact]
    public void CalculateTotal_ReturnsCorrectValue()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Quantity = 2,
                        UnitPrice = 100
                    }
                }
            }
        };

        // Act
        var total = orders.Sum(o => o.TotalAmount);

        // Assert
        Assert.Equal(200, total);
    }

    [Fact]
    public void FilterCompletedOrders_ReturnsOnlyCompleted()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Status = OrderStatus.Completed },
            new Order { Status = OrderStatus.New }
        };

        // Act
        var completed = orders
            .Where(o => o.Status == OrderStatus.Completed)
            .ToList();

        // Assert
        Assert.Single(completed);
    }
}