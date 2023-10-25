namespace Platipus.Wallet.Api.Application.Results.Microgame;

using WithData;

public static class MicrogameResultFactory
{
    public static MicrogameResult Success() => new();

    public static MicrogameResult<TData> Success<TData>(TData data) => new(data);

    public static MicrogameResult Failure(
        MicrogameStatusCode responseStatus,
        Exception? exception = null)
        => new(
            responseStatus,
            exception);

    public static MicrogameResult<TData> Failure<TData>(
        MicrogameStatusCode responseStatus,
        Exception? exception = null)
        => new(
            responseStatus,
            exception);
}