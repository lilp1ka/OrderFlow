using System.Net;
using System.Text;
using OrderFlow.Services;

namespace OrderFlow.Tests;

public class CurrencyServiceTests
{
    [Fact]
    public async Task GetRateAsync_ValidCurrency_ReturnsRate()
    {
        // Arrange
        var handler = new TestHttpMessageHandler();

        handler.Handler = _ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
                {
                  "rates":[
                    {
                      "mid":4.0
                    }
                  ]
                }
                """, Encoding.UTF8, "application/json")
        };

        var httpClient = new HttpClient(handler);

        var service = new CurrencyService(httpClient);

        // Act
        var result = await service.GetRateAsync("USD");

        // Assert
        Assert.Equal(4.0m, result);
    }

    [Fact]
    public async Task GetRateAsync_PLN_ReturnsOne_WithoutApiCall()
    {
        // Arrange
        var handler = new TestHttpMessageHandler();

        var httpClient = new HttpClient(handler);

        var service = new CurrencyService(httpClient);

        // Act
        var result = await service.GetRateAsync("PLN");

        // Assert
        Assert.Equal(1.0m, result);

        Assert.Equal(0, handler.CallCount);
    }

    [Fact]
    public async Task GetRateAsync_NotFound_ReturnsNull()
    {
        // Arrange
        var handler = new TestHttpMessageHandler();

        handler.Handler = _ => new HttpResponseMessage(HttpStatusCode.NotFound);

        var service = new CurrencyService(new HttpClient(handler));

        // Act
        var result = await service.GetRateAsync("XXX");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRateAsync_ServerError_ThrowsException()
    {
        // Arrange
        var handler = new TestHttpMessageHandler();

        handler.Handler = _ =>
            new HttpResponseMessage(HttpStatusCode.InternalServerError);

        var service = new CurrencyService(new HttpClient(handler));

        // Act + Assert
        await Assert.ThrowsAsync<CurrencyServiceException>(() =>
            service.GetRateAsync("USD"));
    }

    [Fact]
    public async Task ConvertAsync_UsdToEur_ReturnsConvertedValue()
    {
        // Arrange
        var handler = new TestHttpMessageHandler();

        handler.Handler = request =>
        {
            if (request.RequestUri!.ToString().Contains("USD"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                          "rates":[
                            {
                              "mid":4.0
                            }
                          ]
                        }
                        """)
                };
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    {
                      "rates":[
                        {
                          "mid":5.0
                        }
                      ]
                    }
                    """)
            };
        };

        var service = new CurrencyService(new HttpClient(handler));

        // Act
        var result = await service.ConvertAsync(100, "USD", "EUR");

        // Assert
        Assert.Equal(80m, result);
    }

    [Fact]
    public async Task GetRateAsync_UsesCorrectUrl()
    {
        // Arrange
        var handler = new TestHttpMessageHandler();

        handler.Handler = _ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
                {
                  "rates":[
                    {
                      "mid":4.0
                    }
                  ]
                }
                """)
        };

        var service = new CurrencyService(new HttpClient(handler));

        // Act
        await service.GetRateAsync("USD");

        // Assert
        Assert.Contains(
            "/api/exchangerates/rates/A/USD/",
            handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetRateAsync_UsesCache_CallsApiOnce()
    {
        // Arrange
        var handler = new TestHttpMessageHandler();

        handler.Handler = _ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
                {
                  "rates":[
                    {
                      "mid":4.0
                    }
                  ]
                }
                """)
        };

        var service = new CurrencyService(new HttpClient(handler));

        // Act
        await service.GetRateAsync("USD");

        await service.GetRateAsync("USD");

        // Assert
        Assert.Equal(1, handler.CallCount);
    }
}