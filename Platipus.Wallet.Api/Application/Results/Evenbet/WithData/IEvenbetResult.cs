namespace Platipus.Wallet.Api.Application.Results.Evenbet.WithData;

using Base.WithData;

public interface IEvenbetResult<out TData> : IBaseResult<EvenbetErrorCode, TData>, IEvenbetResult
{
}