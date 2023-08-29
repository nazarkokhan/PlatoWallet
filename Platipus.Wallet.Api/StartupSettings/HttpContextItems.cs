namespace Platipus.Wallet.Api.StartupSettings;

public static class HttpContextItems
{
    public const string Provider = nameof(Provider);

    public const string RequestObject = nameof(RequestObject);
    public const string ResponseObject = nameof(ResponseObject);

    public const string RequestBodyBytes = nameof(RequestBodyBytes);
    public const string ResponseBodyBytes = nameof(ResponseBodyBytes);

    public const string RequestQueryString = nameof(RequestQueryString);

    public const string BetflagCasinoSecretKey = nameof(BetflagCasinoSecretKey);
    public const string SoftswissAwardSessionId = nameof(SoftswissAwardSessionId);

    public const string OpenboxDecryptedPayloadJsonString = nameof(OpenboxDecryptedPayloadJsonString);
    public const string OpenboxDecryptedPayloadRequestObject = nameof(OpenboxDecryptedPayloadRequestObject);

    public static IBaseRequest GetRequestObject(this HttpContext httpContext)
    {
        return (IBaseRequest)httpContext.Items[RequestObject]!;
    }

    public static byte[] GetRequestBodyBytesItem(this HttpContext httpContext)
    {
        return (byte[])httpContext.Items[RequestBodyBytes]!;
    }
}