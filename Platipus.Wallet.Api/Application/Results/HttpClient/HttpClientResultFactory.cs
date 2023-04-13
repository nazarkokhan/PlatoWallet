namespace Platipus.Wallet.Api.Application.Results.HttpClient;

using HttpData;
using WithData;

public static class HttpClientResultFactory
{
    public static HttpClientResult<TError> Success<TError>(this HttpClientRequest httpRequest) => new(httpRequest);

    public static HttpClientResult<TError> Failure<TError>(this HttpClientRequest httpRequest, TError error)
        => new(httpRequest, error);

    public static HttpClientResult<TError> Failure<TError>(this HttpClientRequest httpRequest, Exception? exception = null)
        => new(httpRequest, default!, exception);


    public static HttpClientResult<TSuccess, TError> Success<TSuccess, TError>(
        this HttpClientRequest httpRequest,
        TSuccess success)
        => new(httpRequest, success);

    public static HttpClientResult<TSuccess, TError> Failure<TSuccess, TError>(this HttpClientRequest httpRequest, TError error)
        => new(httpRequest, error, null);

    public static HttpClientResult<TSuccess, TError> Failure<TSuccess, TError>(
        this HttpClientRequest httpRequest,
        Exception? exception = null)
        => new(httpRequest, default!, exception);
}