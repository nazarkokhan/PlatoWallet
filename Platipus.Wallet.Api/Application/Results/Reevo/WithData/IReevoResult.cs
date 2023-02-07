namespace Platipus.Wallet.Api.Application.Results.Reevo.WithData;

using Base.WithData;

public interface IReevoResult<out TData> : IBaseResult<ReevoErrorCode, TData>, IReevoResult
{
}