namespace Platipus.Wallet.Api.Results.Common.Result.Factories;

public static class BaseResultFactory
{
    public static BaseResult<TError> Success<TError>() => new();

    public static BaseResult<TError, TData> Success<TError, TData>(TData data) => new(data);

    public static BaseResult<TError> Failure<TError>(
        TError errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static BaseResult<TError> Failure<TError>(IBaseResult<TError> result)
        => result.IsFailure
            ? Failure(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static BaseResult<TError, TData> Failure<TError, TData>(
        TError errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static BaseResult<TError, TData> Failure<TError, TData, TSourceData>(IBaseResult<TError, TSourceData> result)
        => result.IsFailure
            ? Failure<TError, TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}