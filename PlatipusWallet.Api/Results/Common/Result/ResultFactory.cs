namespace PlatipusWallet.Api.Results.Common.Result;

using WithData;

public static class ResultFactory
{
    public static Result Success() => new();

    public static Result<T> Success<T>(T data) => new(data);

    public static Result Failure(
        ErrorCode errorCode,
        string description)
        => Failure(errorCode, null, description);
    
    public static Result Failure(
        ErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        => new(errorCode, exception);

    public static Result<T> Failure<T>(
        ErrorCode errorCode,
        string description)
        => Failure<T>(errorCode, null, description);
    
    public static Result<T> Failure<T>(
        ErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        => new(errorCode, exception, description);
}