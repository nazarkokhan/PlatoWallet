namespace Platipus.Wallet.Api.Application.Results.Sw.WithData;

using Base.WithData;

public interface ISwResult<out TData> : IBaseResult<SwErrorCode, TData>, ISwResult
{
}