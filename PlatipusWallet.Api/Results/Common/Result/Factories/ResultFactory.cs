namespace PlatipusWallet.Api.Results.Common.Result.Factories;

using WithData;

public static class ResultFactory
{
    public static Result Success() => new();

    public static Result<TData> Success<TData>(TData data) => new(data);

    public static Result Failure(ErrorCode errorCode) => new(errorCode);

    public static Result<T> Failure<T>(
        ErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);
}