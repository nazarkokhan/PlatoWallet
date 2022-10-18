namespace PlatipusWallet.Api.Results.Common.Result.Factories;

using WithData;

public static class BaseResultFactory
{
    public static BaseResult<TError> Success<TError>() => new();

    public static BaseResult<TError, TData> Success<TError, TData>(TData data) => new(data);

    public static BaseResult<TError> Failure<TError>(
        TError errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static BaseResult<TError, TData> Failure<TError, TData>(
        TError errorCode,
        Exception? exception = null)
        => new(errorCode, exception);
}