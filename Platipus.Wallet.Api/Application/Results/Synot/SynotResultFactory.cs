namespace Platipus.Wallet.Api.Application.Results.Synot;

using WithData;

public static class SynotResultFactory
{
    public static SynotResult Success() => new();

    public static SynotResult<TData> Success<TData>(TData data) => new(data);

    public static SynotResult Failure(
        SynotError error,
        Exception? exception = null)
        => new(
            error,
            exception);

    public static SynotResult Failure(ISynotResult result)
        => result.IsFailure
            ? Failure(
                result.Error,
                result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static SynotResult<TData> Failure<TData>(
        SynotError error,
        Exception? exception = null)
        => new(
            error,
            exception);
}