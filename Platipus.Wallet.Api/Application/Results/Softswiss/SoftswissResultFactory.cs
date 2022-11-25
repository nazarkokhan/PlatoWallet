namespace Platipus.Wallet.Api.Application.Results.Softswiss;

public static class SoftswissResultFactory
{
    public static SoftswissResult Success()
        => new();

    public static SoftswissResult<TData> Success<TData>(TData data)
        => new(data);

    public static SoftswissResult Failure(SoftswissErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static SoftswissResult<TData> Failure<TData>(
        SoftswissErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static SoftswissResult<TData> Failure<TData, TSourceData>(ISoftswissResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}