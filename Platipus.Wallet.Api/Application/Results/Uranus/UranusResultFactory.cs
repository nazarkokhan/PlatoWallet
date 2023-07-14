namespace Platipus.Wallet.Api.Application.Results.Uranus;

using WithData;

public static class UranusResultFactory
{
    public static UranusResult Success() => new();

    public static UranusResult<TData> Success<TData>(TData data) => new(data);

    public static UranusResult Failure(
        UranusErrorCode errorCode, 
        string? balance, 
        string? currency, 
        Exception? exception = null)
        => new(errorCode, balance, currency, exception);

    public static UranusResult Failure(IUranusResult result)
        => result.IsFailure
            ? Failure(result.Error, result.Balance, result.Currency, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static UranusResult<TData> Failure<TData>(
        UranusErrorCode errorCode,
        string? balance = null,
        string? currency = null,
        Exception? exception = null)
        => new(errorCode, balance, currency, exception);

    public static UranusResult<TData> Failure<TData, TSourceData>(
        UranusResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Balance, result.Currency, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}