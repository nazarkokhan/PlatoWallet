namespace PlatipusWallet.Api.Results.Common.Result.Factories;

using WithData;

public static class ResultFactory
{
    public static Result Success() => new();

    public static Result<TData> Success<TData>(TData data) => new(data);

    public static Result Failure(ErrorCode errorCode, Exception? exception = null) => new(errorCode, exception);

    public static Result Failure(IResult result)
        => result.IsFailure
            ? Failure(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static Result<TData> Failure<TData>(
        ErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static Result<TData> Failure<TData, TSourceData>(IResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}