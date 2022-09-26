namespace PlatipusWallet.Api.Options;

// record CurrencyMetadata(CurrencyType CurrencyType);

public class SupportedCurrenciesOptions
{
    public HashSet<string> Fiat { get; set; }

    public HashSet<string> Crypto { get; set; }
}