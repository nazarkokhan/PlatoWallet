namespace Platipus.Wallet.Api.Application.Results.Betflag;

using WithData;

public static class BetflagResultFactory
{
    public static BetflagResult Success()
        => new();

    public static BetflagResult<TData> Success<TData>(TData data)
        => new(data);

    public static BetflagResult Failure(BetflagErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static BetflagResult<TData> Failure<TData>(
        BetflagErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static BetflagResult<TData> Failure<TData, TSourceData>(IBetflagResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}