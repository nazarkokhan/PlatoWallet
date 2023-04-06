namespace Platipus.Wallet.Api.Application.Results.EmaraPlay;

using WithData;

public static class EmaraPlayResultFactory
{
    public static EmaraPlayResult Success() => new();

    public static EmaraPlayResult<TData> Success<TData>(TData data) => new(data);

    public static EmaraPlayResult Failure(EmaraPlayErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static EmaraPlayResult Failure(IEmaraPlayResult result)
        => result.IsFailure
            ? Failure(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static EmaraPlayResult<TData> Failure<TData>(
        EmaraPlayErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static EmaraPlayResult<TData> Failure<TData, TSourceData>(IEmaraPlayResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}