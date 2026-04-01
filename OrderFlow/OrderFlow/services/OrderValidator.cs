using OrderFlow.Models;

namespace OrderFlow.Services;

public delegate bool ValidationRule(Order order, out string errorMessage);

public static class OrderValidator
{
    
    public static bool HasItems(Order order, out string errorMessage)
    {
        if(order.Items.Count > 0) { errorMessage=""; return true; }
        errorMessage="Order has no items"; return false;
    }

    public static bool QuantitiesPositive(Order order, out string errorMessage)
    {
        if(order.Items.All(i=>i.Quantity>0)) { errorMessage=""; return true; }
        errorMessage="Some item has zero or negative quantity"; return false;
    }

    public static bool TotalUnderLimit(Order order, out string errorMessage)
    {
        if(order.TotalAmount <= 1000) { errorMessage=""; return true; }
        errorMessage="Total exceeds 1000"; return false;
    }

    
    public static Func<Order,bool> NotCancelled = o => o.Status != OrderStatus.Cancelled;
    public static Func<Order,bool> DateNotFuture = o => o.Date <= DateTime.Now;

    public static bool ValidateAll(Order order, out List<string> errors)
    {
        errors = new List<string>();
        var rules = new ValidationRule[]{ HasItems, QuantitiesPositive, TotalUnderLimit };
        foreach(var r in rules)
            if(!r(order, out string msg) && !string.IsNullOrEmpty(msg))
                errors.Add(msg);
        var funcs = new Func<Order,bool>[] { NotCancelled, DateNotFuture };
        foreach(var f in funcs)
            if(!f(order)) errors.Add("Func rule failed");
        return errors.Count == 0;
    }
}