namespace Platipus.Wallet.Api.Application.Results.ISoftBet.WithData;

using Base.WithData;

public interface ISoftBetResult<out TData> : IBaseResult<SoftBetErrorMessage, TData>, ISoftBetResult
{
}