namespace Platipus.Wallet.Api.Extensions;

public static class NemesisHeaders
{
    public const string XIntegrationToken = "X-Integration-Token";

    public static string? GetXIntegrationToken(this IHeaderDictionary headers)
    {
        return headers.TryGetValue(XIntegrationToken, out var xRequestSign) ? xRequestSign.ToString() : null;
    }
}