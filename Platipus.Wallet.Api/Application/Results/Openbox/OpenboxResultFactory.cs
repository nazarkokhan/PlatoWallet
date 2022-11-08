namespace Platipus.Wallet.Api.Application.Results.Openbox;

using WithData;

public static class OpenboxResultFactory
{
    public static OpenboxResult Success() => new();

    public static OpenboxResult<TData> Success<TData>(TData data) => new(data);

    public static OpenboxResult Failure(OpenboxErrorCode errorCode, Exception? exception = null) => new(errorCode, exception);

    public static OpenboxResult<T> Failure<T>(
        OpenboxErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);
    
    public static OpenboxResult<TData> Failure<TData, TSourceData>(IOpenboxResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}