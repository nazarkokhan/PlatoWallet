namespace Platipus.Wallet.Api.Application.Results.Reevo;

using WithData;

public static class ReevoResultFactory
{
    public static ReevoResult Success() => new();

    public static ReevoResult<TData> Success<TData>(TData data) => new(data);

    public static ReevoResult Failure(ReevoErrorCode errorCode, Exception? exception = null) => new(errorCode, exception);

    public static ReevoResult<TData> Failure<TData>(
        ReevoErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static ReevoResult<TData> Failure<TData, TSourceData>(IReevoResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}