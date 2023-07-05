namespace Platipus.Wallet.Api.Application.Results.Evoplay;

using WithData;

public static class EvoplayResultFactory
{
    public static EvoplayResult Success() => new();

    public static EvoplayResult<TData> Success<TData>(TData data) => new(data);

    public static EvoplayResult Failure(
        EvoplayErrorCode errorCode, 
        string? balance, 
        string? currency, 
        Exception? exception = null)
        => new(errorCode, balance, currency, exception);

    public static EvoplayResult Failure(IEvoplayResult result)
        => result.IsFailure
            ? Failure(result.Error, result.Balance, result.Currency, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static EvoplayResult<TData> Failure<TData>(
        EvoplayErrorCode errorCode,
        string? balance = null,
        string? currency = null,
        Exception? exception = null)
        => new(errorCode, balance, currency, exception);

    public static EvoplayResult<TData> Failure<TData, TSourceData>(
        EvoplayResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Balance, result.Currency, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}