using OrderFlow.Models;
using OrderFlow.Services;

namespace OrderFlow.Tests;

public class DiscountCalculatorTests
{
    [Fact]
    public void CalculateDiscount_RegularCustomerSmallOrder_ReturnsZero()
    {
        // Arrange
        var calculator = new DiscountCalculator();

        var order = CreateOrder(false, 500);

        // Act
        var result = calculator.CalculateDiscount(order);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateDiscount_VipCustomer_Returns10Percent()
    {
        // Arrange
        var calculator = new DiscountCalculator();

        var order = CreateOrder(true, 1000);

        // Act
        var result = calculator.CalculateDiscount(order);

        // Assert
        Assert.Equal(100m, result);
    }

    [Fact]
    public void CalculateDiscount_HighValueOrder_Returns5Percent()
    {
        // Arrange
        var calculator = new DiscountCalculator();

        var order = CreateOrder(false, 2000);

        // Act
        var result = calculator.CalculateDiscount(order);

        // Assert
        Assert.Equal(100m, result);
    }

    [Fact]
    public void CalculateDiscount_VipAndHighValue_Returns15Percent()
    {
        // Arrange
        var calculator = new DiscountCalculator();

        var order = CreateOrder(true, 2000);

        // Act
        var result = calculator.CalculateDiscount(order);

        // Assert
        Assert.Equal(300m, result);
    }

    [Fact]
    public void CalculateDiscount_VipOver5000_Returns20Percent()
    {
        // Arrange
        var calculator = new DiscountCalculator();

        var order = CreateOrder(true, 6000);

        // Act
        var result = calculator.CalculateDiscount(order);

        // Assert
        Assert.Equal(1200m, result);
    }

    [Fact]
    public void CalculateDiscount_DiscountCannotExceed25Percent()
    {
        // Arrange
        var calculator = new DiscountCalculator();

        var order = CreateOrder(true, 10000);

        // Act
        var result = calculator.CalculateDiscount(order);

        // Assert
        Assert.True(result <= 2500m);
    }

    [Theory]
    [InlineData(false, 500, 0)]
    [InlineData(true, 1000, 100)]
    [InlineData(false, 2000, 100)]
    [InlineData(true, 2000, 300)]
    public void CalculateDiscount_MultipleCases_ReturnsExpectedResult(
        bool isVip,
        decimal total,
        decimal expected)
    {
        // Arrange
        var calculator = new DiscountCalculator();

        var order = CreateOrder(isVip, total);

        // Act
        var result = calculator.CalculateDiscount(order);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateDiscount_EmptyOrder_ReturnsZero()
    {
        // Arrange
        var calculator = new DiscountCalculator();

        var order = new Order
        {
            Customer = new Customer
            {
                IsVip = false
            },
            Items = []
        };

        // Act
        var result = calculator.CalculateDiscount(order);

        // Assert
        Assert.Equal(0m, result);
    }

    private static Order CreateOrder(bool isVip, decimal total)
    {
        return new Order
        {
            Customer = new Customer
            {
                IsVip = isVip
            },

            Items =
            [
                new OrderItem
                {
                    Quantity = 1,
                    UnitPrice = total
                }
            ]
        };
    }
}