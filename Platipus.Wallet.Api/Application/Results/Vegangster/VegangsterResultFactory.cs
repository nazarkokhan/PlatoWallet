namespace Platipus.Wallet.Api.Application.Results.Vegangster;

using WithData;

public static class VegangsterResultFactory
{
    public static VegangsterResult Success() => new();

    public static VegangsterResult<TData> Success<TData>(TData data) => new(data);

    public static VegangsterResult Failure(
        VegangsterResponseStatus responseStatus,
        Exception? exception = null)
        => new(
            responseStatus,
            exception);

    public static VegangsterResult Failure(IVegangsterResult result)
        => result.IsFailure
            ? Failure(
                result.Error,
                result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static VegangsterResult<TData> Failure<TData>(
        VegangsterResponseStatus responseStatus,
        Exception? exception = null)
        => new(
            responseStatus,
            exception);
}