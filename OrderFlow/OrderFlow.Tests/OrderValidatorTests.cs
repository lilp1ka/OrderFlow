using OrderFlow.Models;
using OrderFlow.Services;

namespace OrderFlow.Tests;

public class OrderValidatorTests
{
    [Fact]
    public void HasItems_EmptyOrder_ReturnsFalse()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>()
        };

        // Act
        var result = OrderValidator.HasItems(order, out var error);

        // Assert
        Assert.False(result);
        Assert.Equal("Order has no items", error);
    }

    [Fact]
    public void QuantitiesPositive_NegativeQuantity_ReturnsFalse()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { Quantity = -1 }
            }
        };

        // Act
        var result = OrderValidator.QuantitiesPositive(order, out var error);

        // Assert
        Assert.False(result);
        Assert.Equal("Some item has zero or negative quantity", error);
    }

    [Fact]
    public void TotalUnderLimit_TooLarge_ReturnsFalse()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    Quantity = 10,
                    UnitPrice = 200
                }
            }
        };

        // Act
        var result = OrderValidator.TotalUnderLimit(order, out var error);

        // Assert
        Assert.False(result);
        Assert.Equal("Total exceeds 1000", error);
    }

    [Fact]
    public void DateNotFuture_FutureDate_ReturnsFalse()
    {
        // Arrange
        var order = new Order
        {
            Date = DateTime.Now.AddDays(1)
        };

        // Act
        var result = OrderValidator.DateNotFuture(order);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(OrderStatus.Cancelled, false)]
    [InlineData(OrderStatus.New, true)]
    [InlineData(OrderStatus.Completed, true)]
    public void NotCancelled_CheckStatuses_ReturnsExpected(
        OrderStatus status,
        bool expected)
    {
        // Arrange
        var order = new Order
        {
            Status = status
        };

        // Act
        var result = OrderValidator.NotCancelled(order);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ValidateAll_InvalidOrder_ReturnsErrors()
    {
        // Arrange
        var order = new Order
        {
            Date = DateTime.Now.AddDays(1),
            Status = OrderStatus.Cancelled,
            Items = new List<OrderItem>()
        };

        // Act
        var result = OrderValidator.ValidateAll(order, out var errors);

        // Assert
        Assert.False(result);
        Assert.NotEmpty(errors);
    }
}