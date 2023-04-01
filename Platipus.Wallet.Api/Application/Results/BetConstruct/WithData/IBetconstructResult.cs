namespace Platipus.Wallet.Api.Application.Results.BetConstruct.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public interface IBetconstructResult<out TData> : IBaseResult<BetconstructErrorCode, TData>, IBetconstructResult
{
}