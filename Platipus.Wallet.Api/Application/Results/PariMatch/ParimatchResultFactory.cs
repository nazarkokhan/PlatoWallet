namespace Platipus.Wallet.Api.Application.Results.PariMatch;

using WithData;

public static class ParimatchResultFactory
{
    public static ParimatchResult Success()
        => new();

    public static ParimatchResult<TData> Success<TData>(TData data)
        => new(data);

    public static ParimatchResult Failure(ParimatchErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static ParimatchResult<TData> Failure<TData>(
        ParimatchErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static ParimatchResult<TData> Failure<TData, TSourceData>(IParimatchResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}