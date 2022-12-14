namespace Platipus.Wallet.Api.Application.Results.GamesGlobal.WithData;

using Base.WithData;

public interface IGamesGlobalResult<out TData> : IBaseResult<GamesGlobalErrorCode, TData>, IGamesGlobalResult
{
}