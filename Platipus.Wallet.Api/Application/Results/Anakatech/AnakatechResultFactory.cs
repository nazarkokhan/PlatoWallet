namespace Platipus.Wallet.Api.Application.Results.Anakatech;

using WithData;

public static class AnakatechResultFactory
{
    public static AnakatechResult Success() => new();

    public static AnakatechResult<TData> Success<TData>(TData data) => new(data);

    public static AnakatechResult Failure(
        AnakatechErrorCode errorCode,
        int balance,
        bool success,
        Exception? exception = null)
        => new(
            errorCode,
            balance,
            success,
            exception);

    public static AnakatechResult Failure(IAnakatechResult result)
        => result.IsFailure
            ? Failure(
                result.Error,
                result.Balance,
                result.Success,
                result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static AnakatechResult<TData> Failure<TData>(
        AnakatechErrorCode errorCode,
        int balance = default,
        bool success = false,
        Exception? exception = null)
        => new(
            errorCode,
            balance,
            success,
            exception);

    public static AnakatechResult<TData> Failure<TData, TSourceData>(AnakatechResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(
                result.Error,
                result.Balance,
                result.Success,
                result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}