namespace Platipus.Wallet.Api.Application.Results.EmaraPlay;

using WithData;

public static class EmaraPlayResultFactory
{
    public static EmaraPlayResult Success() => new();

    public static EmaraPlayResult<TData> Success<TData>(TData data) => new(data);

    public static EmaraPlayResult Failure(EmaraPlayErrorCode errorCode, long? balance = null, Exception? exception = null)
        => new(errorCode, balance, exception);

    public static EmaraPlayResult Failure(IEmaraPlayResult result)
        => result.IsFailure
            ? Failure(result.Error, result.Balance, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static EmaraPlayResult<TData> Failure<TData>(
        EmaraPlayErrorCode errorCode,
        Exception? exception = null,
        long? balance = null)
        => new(errorCode, balance, exception);

    public static EmaraPlayResult<TData> Failure<TData, TSourceData>(IEmaraPlayResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception, result.Balance)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}