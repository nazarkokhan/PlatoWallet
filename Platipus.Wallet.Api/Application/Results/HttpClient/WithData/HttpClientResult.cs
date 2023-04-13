namespace Platipus.Wallet.Api.Application.Results.HttpClient.WithData;

using HttpData;
using Platipus.Wallet.Api.Application.Results.Base.WithData;

public record HttpClientResult<TSuccess, TError> : BaseResult<TError, TSuccess>, IHttpClientResult<TSuccess, TError>
{
    public HttpClientResult(HttpClientRequest httpRequest, TSuccess data)
        : base(data)
    {
        HttpRequest = httpRequest;
    }

    public HttpClientResult(
        HttpClientRequest httpRequest,
        TError error,
        Exception? exception)
        : base(error, exception)
    {
        HttpRequest = httpRequest;
    }

    public HttpClientRequest HttpRequest { get; set; }
}