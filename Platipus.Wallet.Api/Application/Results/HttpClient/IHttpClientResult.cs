namespace Platipus.Wallet.Api.Application.Results.HttpClient;

using System.Text.Json.Serialization;
using Base;
using HttpData;

public interface IHttpClientResult<out TError> : IBaseResult<TError>
{
    public HttpClientRequest HttpRequest { get; set; }
}