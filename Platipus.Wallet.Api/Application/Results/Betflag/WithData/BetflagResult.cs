namespace Platipus.Wallet.Api.Application.Results.Betflag.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public record BetflagResult<TData> : BaseResult<BetflagErrorCode, TData>, IBetflagResult<TData>
{
    public BetflagResult(TData data)
        : base(data)
    {
    }

    public BetflagResult(
        BetflagErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}