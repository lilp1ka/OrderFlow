using OrderFlow.Models;

namespace OrderFlow.Services;

public class ExternalServiceSimulator
{
    public async Task<bool> CheckInventoryAsync(Product product)
    {
        await Task.Delay(Random.Shared.Next(500, 1500));
        return true;
    }

    public async Task<bool> ValidatePaymentAsync(Order order)
    {
        await Task.Delay(Random.Shared.Next(1000, 2000));
        return true;
    }

    public async Task<decimal> CalculateShippingAsync(Order order)
    {
        await Task.Delay(Random.Shared.Next(300, 800));
        return 10;
    }
}