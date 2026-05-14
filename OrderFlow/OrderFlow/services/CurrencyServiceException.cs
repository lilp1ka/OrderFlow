namespace OrderFlow.Services;

public class CurrencyServiceException : Exception
{
    public CurrencyServiceException(string message)
        : base(message)
    {
    }
}