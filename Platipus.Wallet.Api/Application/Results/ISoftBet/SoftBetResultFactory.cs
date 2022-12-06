namespace Platipus.Wallet.Api.Application.Results.ISoftBet;

using WithData;

public static class SoftBetResultFactory
{
    public static SoftBetResult Success()
        => new();

    public static SoftBetResult<TData> Success<TData>(TData data)
        => new(data);

    public static SoftBetResult Failure(SoftBetError errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static SoftBetResult<TData> Failure<TData>(
        SoftBetError errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static SoftBetResult<TData> Failure<TData, TSourceData>(ISoftBetResult<TSourceData> pswResult)
        => pswResult.IsFailure
            ? Failure<TData>(pswResult.ErrorCode, pswResult.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(pswResult));
}