namespace Platipus.Wallet.Api.Application.Results.HttpClient;

using Base;
using HttpData;

public interface IHttpClientResult<out TError> : IBaseResult<TError>
{
    public HttpClientRequest HttpRequest { get; set; }
}