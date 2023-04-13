namespace Platipus.Wallet.Api.Application.Results.GamesGlobal;

using WithData;

public static class GamesGlobalResultFactory
{
    public static GamesGlobalResult Success() => new();

    public static GamesGlobalResult<TData> Success<TData>(TData data) => new(data);

    public static GamesGlobalResult Failure(GamesGlobalErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static GamesGlobalResult<TData> Failure<TData>(
        GamesGlobalErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static GamesGlobalResult<TData> Failure<TData, TSourceData>(IGamesGlobalResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}