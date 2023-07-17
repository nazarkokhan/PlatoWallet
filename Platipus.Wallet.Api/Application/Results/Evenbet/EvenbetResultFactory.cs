namespace Platipus.Wallet.Api.Application.Results.Evenbet;

using WithData;

public static class EvenbetResultFactory
{
    public static EvenbetResult Success() => new();

    public static EvenbetResult<TData> Success<TData>(TData data) => new(data);

    public static EvenbetResult Failure(
        EvenbetErrorCode errorCode,
        int balance,
        string timestamp,
        Exception? exception = null)
        => new(
            errorCode,
            balance,
            timestamp,
            exception);

    public static EvenbetResult Failure(IEvenbetResult result)
        => result.IsFailure
            ? Failure(
                result.Error,
                result.Balance,
                result.Timestamp,
                result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static EvenbetResult<TData> Failure<TData>(
        EvenbetErrorCode errorCode,
        int balance = default,
        string timestamp = "",
        Exception? exception = null)
        => new(
            errorCode,
            balance,
            timestamp,
            exception);

    public static EvenbetResult<TData> Failure<TData, TSourceData>(EvenbetResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(
                result.Error,
                result.Balance,
                result.Timestamp,
                result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}