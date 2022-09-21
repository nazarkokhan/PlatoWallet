namespace PlatipusWallet.Api.Options;

using Enums;

record CurrencyMetadata(CurrencyType CurrencyType);

public class SupportedCurrenciesOptions
{
    
    public Dictionary<string, CurrencyType> Currencies { get; init; }
    
    public HashSet<string> Fiat { get; init; }
    
    public HashSet<string> Crypto { get; init; }
}