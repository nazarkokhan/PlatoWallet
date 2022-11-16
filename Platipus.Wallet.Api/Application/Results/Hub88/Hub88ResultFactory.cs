namespace Platipus.Wallet.Api.Application.Results.Hub88;

using WithData;

public static class Hub88ResultFactory
{
    public static Hub88Result Success()
        => new();

    public static Hub88Result<TData> Success<TData>(TData data)
        => new(data);

    public static Hub88Result Failure(Hub88ErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static Hub88Result<TData> Failure<TData>(
        Hub88ErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static Hub88Result<TData> Failure<TData, TSourceData>(IHub88Result<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}