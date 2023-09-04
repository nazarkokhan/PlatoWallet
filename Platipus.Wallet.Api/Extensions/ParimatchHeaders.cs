namespace Platipus.Wallet.Api.Extensions;

public static class ParimatchHeaders
{
    public const string XHubConsumer = "X-Hub-Consumer";

    public static string? GetXHubConsumer(this IHeaderDictionary headers)
    {
        return headers.TryGetValue(XHubConsumer, out var xHubConsumer) ? xHubConsumer.ToString() : null;
    }
}