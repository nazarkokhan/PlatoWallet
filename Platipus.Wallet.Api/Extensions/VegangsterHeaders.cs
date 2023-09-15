namespace Platipus.Wallet.Api.Extensions;

public static class VegangsterHeaders
{
    public const string XApiSignature = "X-API-Signature";

    public static string? GetXApiSignature(this IHeaderDictionary headers)
    {
        return headers.TryGetValue(XApiSignature, out var xApiSignature) ? xApiSignature.ToString() : null;
    }
}