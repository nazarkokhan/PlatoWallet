namespace Platipus.Wallet.Api.Application.Results.BetConstruct.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public interface IBetConstructResult<out TData> : IBaseResult<BetConstructErrorCode, TData>, IBetConstructResult
{
}