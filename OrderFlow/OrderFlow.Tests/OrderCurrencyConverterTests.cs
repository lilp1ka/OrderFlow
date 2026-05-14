using Moq;
using OrderFlow.Models;
using OrderFlow.Services;

namespace OrderFlow.Tests;

public class OrderCurrencyConverterTests
{
    [Fact]
    public async Task ConvertOrderTotalAsync_ReturnsConvertedAmount()
    {
        // Arrange
        var currencyServiceMock = new Mock<ICurrencyService>();

        currencyServiceMock
            .Setup(x => x.ConvertAsync(100m, "PLN", "USD"))
            .ReturnsAsync(25m);

        var converter =
            new OrderCurrencyConverter(currencyServiceMock.Object);

        var order = new Order
        {
            Items =
            [
                new OrderItem
                {
                    Quantity = 1,
                    UnitPrice = 100
                }
            ]
        };

        // Act
        var result =
            await converter.ConvertOrderTotalAsync(order, "USD");

        // Assert
        Assert.Equal(25m, result);
    }

    [Fact]
    public async Task ConvertOrderTotalAsync_CallsCurrencyService()
    {
        // Arrange
        var currencyServiceMock = new Mock<ICurrencyService>();

        currencyServiceMock
            .Setup(x => x.ConvertAsync(It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(50m);

        var converter =
            new OrderCurrencyConverter(currencyServiceMock.Object);

        var order = new Order
        {
            Items =
            [
                new OrderItem
                {
                    Quantity = 1,
                    UnitPrice = 200
                }
            ]
        };

        // Act
        await converter.ConvertOrderTotalAsync(order, "EUR");

        // Assert
        currencyServiceMock.Verify(x =>
                x.ConvertAsync(200m, "PLN", "EUR"),
            Times.Once);
    }
}