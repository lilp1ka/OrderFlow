using System.Text.Json;

namespace OrderFlow.Services;

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;

    private readonly Dictionary<string, decimal> _cache = new();

    public CurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal?> GetRateAsync(string currencyCode)
    {
        currencyCode = currencyCode.ToUpper();

        if (currencyCode == "PLN")
            return 1.0m;

        if (_cache.ContainsKey(currencyCode))
            return _cache[currencyCode];

        var url =
            $"https://api.nbp.pl/api/exchangerates/rates/A/{currencyCode}/?format=json";

        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new CurrencyServiceException(
                $"NBP API error: {response.StatusCode}");

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        decimal rate = document
            .RootElement
            .GetProperty("rates")[0]
            .GetProperty("mid")
            .GetDecimal();

        _cache[currencyCode] = rate;

        return rate;
    }

    public async Task<decimal> ConvertAsync(
        decimal amount,
        string fromCurrency,
        string toCurrency)
    {
        var fromRate = await GetRateAsync(fromCurrency);
        var toRate = await GetRateAsync(toCurrency);

        if (fromRate == null || toRate == null)
            throw new CurrencyServiceException("Currency not found");

        decimal amountInPln = amount * fromRate.Value;

        return amountInPln / toRate.Value;
    }
}