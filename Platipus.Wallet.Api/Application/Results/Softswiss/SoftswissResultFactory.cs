namespace Platipus.Wallet.Api.Application.Results.Softswiss;

public static class SoftswissResultFactory
{
    public static SoftswissResult Success()
        => new();

    public static SoftswissResult<TData> Success<TData>(TData data)
        => new(data);

    public static SoftswissResult Failure(SoftswissErrorCode errorCode, long? balance = null, Exception? exception = null)
        => new(errorCode, balance, exception);

    public static SoftswissResult Failure(ISoftswissResult result)
        => result.IsFailure
            ? Failure(result.ErrorCode, result.Balance, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static SoftswissResult<TData> Failure<TData>(
        SoftswissErrorCode errorCode,
        long? balance = null,
        Exception? exception = null)
        => new(errorCode, balance, exception);

    public static SoftswissResult<TData> Failure<TData, TSourceData>(ISoftswissResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Balance, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}