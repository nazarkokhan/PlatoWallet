namespace Platipus.Wallet.Api.Extensions;

public static class UranusHeaders
{
    public const string XSignature = "X-Signature";

    public static string? GetXSignature(this IHeaderDictionary headers)
    {
        return headers.TryGetValue(XSignature, out var xHubConsumer) ? xHubConsumer.ToString() : null;
    }
}