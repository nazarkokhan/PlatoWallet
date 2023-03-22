namespace Platipus.Wallet.Api.StartupSettings.Options;

public class SoftswissCurrenciesOptions
{
    public Dictionary<string, long> CountryIndexes { get; set; } = null!;

    public long GetSumOut(string currency, decimal sum)
    {
        var multiplier = GetMultiplier(currency);

        return (long)(sum * multiplier);
    }

    public decimal GetSumIn(string currency, decimal sum)
    {
        var multiplier = GetMultiplier(currency);

        return sum / multiplier;
    }

    public long GetMultiplier(string currency)
    {
        var valueFound = CountryIndexes.TryGetValue(currency, out var multiplier);

        return valueFound
            ? multiplier
            : 100;
    }
}