namespace Platipus.Wallet.Api.Application.Results.Base.WithData;

public interface IBaseResult<out TError, out TData> : IBaseResult<TError>
{
    TData Data { get; }
}