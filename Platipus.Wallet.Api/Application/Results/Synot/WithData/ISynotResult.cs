namespace Platipus.Wallet.Api.Application.Results.Synot.WithData;

using Base.WithData;

public interface ISynotResult<out TData> : IBaseResult<SynotError, TData>, ISynotResult
{
}