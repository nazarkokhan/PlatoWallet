namespace Platipus.Wallet.Api.Application.Results.BetConstruct.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public record BetConstructResult<TData> : BaseResult<BetConstructErrorCode, TData>, IBetConstructResult<TData>
{
    public BetConstructResult(TData data)
        : base(data)
    {
    }

    public BetConstructResult(
        BetConstructErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}