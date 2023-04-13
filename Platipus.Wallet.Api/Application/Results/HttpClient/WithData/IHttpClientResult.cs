namespace Platipus.Wallet.Api.Application.Results.HttpClient.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public interface IHttpClientResult<out TSuccess, out TError> : IBaseResult<TError, TSuccess>, IHttpClientResult<TError>
{
}