namespace Platipus.Wallet.Api.StartupSettings.Options;

public class SoftswissCurrenciesOptions
{
    public Dictionary<string, long> CountryIndexes { get; set; } = null!;

    public long GetSumOut(string currency, decimal balance)
    {
        var multiplier = GetMultiplier(currency);

        return (long)(balance * multiplier);
    }

    public long GetSumIn(string currency, decimal balance)
    {
        var multiplier = GetMultiplier(currency);

        return (long)(balance / multiplier);
    }

    public long GetMultiplier(string currency)
    {
        var valueFound = CountryIndexes.TryGetValue(currency, out var multiplier);

        return valueFound
            ? multiplier
            : 100;
    }
}