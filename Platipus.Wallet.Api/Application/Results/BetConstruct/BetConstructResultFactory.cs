namespace Platipus.Wallet.Api.Application.Results.BetConstruct;

using WithData;

public static class BetConstructResultFactory
{
    public static BetConstructResult Success()
        => new();

    public static BetConstructResult<TData> Success<TData>(TData data)
        => new(data);

    public static BetConstructResult Failure(BetConstructErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static BetConstructResult<TData> Failure<TData>(
        BetConstructErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static BetConstructResult<TData> Failure<TData, TSourceData>(IBetConstructResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}