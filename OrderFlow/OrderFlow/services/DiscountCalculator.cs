using OrderFlow.Models;

namespace OrderFlow.Services;

public class DiscountCalculator
{
    private const decimal VipDiscountRate = 0.10m;
    private const decimal HighValueRate = 0.05m;
    private const decimal ExtraVipRate = 0.05m;
    private const decimal MaxDiscountRate = 0.25m;

    public decimal CalculateDiscount(Order order)
    {
        decimal total = order.TotalAmount;

        decimal discountRate = 0m;

        if (order.Customer?.IsVip == true)
            discountRate += VipDiscountRate;

        if (total > 1000)
            discountRate += HighValueRate;

        if (order.Customer?.IsVip == true && total > 5000)
            discountRate += ExtraVipRate;

        if (discountRate > MaxDiscountRate)
            discountRate = MaxDiscountRate;

        return total * discountRate;
    }
}