namespace Platipus.Wallet.Api.Application.Results.Betflag.WithData;

using Base.WithData;

public interface IBetflagResult<out TData> : IBaseResult<BetflagErrorCode, TData>, IBetflagResult
{
}