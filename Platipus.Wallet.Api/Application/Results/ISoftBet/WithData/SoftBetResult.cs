namespace Platipus.Wallet.Api.Application.Results.ISoftBet.WithData;

using Base.WithData;

public record SoftBetResult<TData> : BaseResult<SoftBetErrorMessage, TData>, ISoftBetResult<TData>
{
    public SoftBetResult(TData data)
        : base(data)
    {
    }

    public SoftBetResult(
        SoftBetErrorMessage errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}