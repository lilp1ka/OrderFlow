using OrderFlow.Models;

namespace OrderFlow.Services;

public class DiscountCalculator
{
    public decimal CalculateDiscount(Order order)
    {
        decimal total = order.TotalAmount;

        decimal discountPercent = 0;

        if (order.Customer.IsVip)
            discountPercent += 10;

        if (total > 1000)
            discountPercent += 5;

        if (order.Customer.IsVip && total > 5000)
            discountPercent += 5;

        if (discountPercent > 25)
            discountPercent = 25;

        return total * discountPercent / 100m;
    }
}