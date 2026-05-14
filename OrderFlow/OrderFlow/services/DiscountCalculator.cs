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

        if (IsVip(order))
            discountRate += VipDiscountRate;

        if (discountRate > MaxDiscountRate)
            discountRate = MaxDiscountRate;

        return total * discountRate;
    }
    
    private static bool IsVip(Order order)
    {
        return order.Customer?.IsVip == true;
    }
}