namespace Platipus.Wallet.Api.StartupSettings;

public static class HttpContextItems
{
    public const string RequestObject = nameof(RequestObject);
    public const string ResponseObject = nameof(ResponseObject);

    public const string RequestBodyBytes = nameof(RequestBodyBytes);
    public const string ResponseBodyBytes = nameof(ResponseBodyBytes);

    public const string BetflagCasinoSecretKey = nameof(BetflagCasinoSecretKey);

    public static byte[] GetRequestBodyBytesItem(this HttpContext httpContext)
    {
        return (byte[])httpContext.Items[RequestBodyBytes]!;
    }
}