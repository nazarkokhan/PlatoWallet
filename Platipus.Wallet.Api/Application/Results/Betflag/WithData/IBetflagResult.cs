namespace Platipus.Wallet.Api.Application.Results.Betflag.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;
using Platipus.Wallet.Api.Application.Results.PariMatch;

public interface IBetflagResult<out TData> : IBaseResult<BetflagErrorCode, TData>, IBetflagResult
{
}