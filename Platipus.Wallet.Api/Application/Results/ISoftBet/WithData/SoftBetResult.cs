namespace Platipus.Wallet.Api.Application.Results.ISoftBet.WithData;

using Base.WithData;

public record SoftBetResult<TData> : BaseResult<SoftBetError, TData>, ISoftBetResult<TData>
{
    public SoftBetResult(TData data)
        : base(data)
    {
    }

    public SoftBetResult(
        SoftBetError errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}