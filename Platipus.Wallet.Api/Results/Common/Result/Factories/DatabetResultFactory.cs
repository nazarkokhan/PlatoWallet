namespace Platipus.Wallet.Api.Results.Common.Result.Factories;

using WithData;

public static class DatabetResultFactory
{
    public static DatabetResult Success() => new();

    public static DatabetResult<TData> Success<TData>(TData data) => new(data);

    public static DatabetResult Failure(DatabetErrorCode errorCode, Exception? exception = null) => new(errorCode, exception);

    public static DatabetResult Failure(IDatabetResult result)
        => result.IsFailure
            ? Failure(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static DatabetResult<T> Failure<T>(
        DatabetErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);
    
    public static DatabetResult<TData> Failure<TData, TSourceData>(IDatabetResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}