namespace Platipus.Wallet.Api.Application.Results.HttpClient;

using System.Text.Json.Serialization;
using HttpData;
using Base;

public record HttpClientResult<TError> : BaseResult<TError>, IHttpClientResult<TError>
{
    public HttpClientResult(HttpClientRequest httpRequest)
    {
        HttpRequest = httpRequest;
    }

    public HttpClientResult(
        HttpClientRequest httpRequest,
        TError errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
        HttpRequest = httpRequest;
    }

    public HttpClientRequest HttpRequest { get; set; }
}