namespace PlatipusWallet.Api.Filters;

public static class PswHeaders
{
    public const string XRequestSign = "X-REQUEST-SIGN";

    public static string? GetXRequestSign(this IHeaderDictionary headers)
    {
        return headers.TryGetValue(XRequestSign, out var xRequestSign) ? xRequestSign.ToString() : null;
    }
    
}