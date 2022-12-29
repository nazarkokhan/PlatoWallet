namespace Platipus.Wallet.Api.Application.Results.Everymatrix.WithData;

using Base.WithData;

public interface IEverymatrixResult<out TData> : IBaseResult<EverymatrixErrorCode, TData>, IEverymatrixResult
{
}