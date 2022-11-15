namespace Platipus.Wallet.Api.Extensions;

public static class Hub88Headers
{
    public const string XHub88Signature = "X-Hub88-Signature";

    public static string? GetXHub88Signature(this IHeaderDictionary headers)
    {
        return headers.TryGetValue(XHub88Signature, out var xHub88Signature) ? xHub88Signature.ToString() : null;
    }
}