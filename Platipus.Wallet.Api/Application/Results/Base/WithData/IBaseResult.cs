namespace Platipus.Wallet.Api.Application.Results.Base.WithData;

public interface IBaseResult<out TError, out TData> : IBaseResult<TError>
{
    TData Data { get; }
    
    IBaseResult<TNewError, TNewData> ConvertResult<TNewError, TNewData>(TNewError error, TNewData data);
}