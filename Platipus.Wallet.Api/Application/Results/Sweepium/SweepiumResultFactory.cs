using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;

namespace Platipus.Wallet.Api.Application.Results.Sweepium;

public static class SweepiumResultFactory
{
    public static SweepiumResult Success()
        => new();

    public static SweepiumResult<TData> Success<TData>(TData data)
        => new(data);

    public static SweepiumResult Failure(SweepiumErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static SweepiumResult<TData> Failure<TData>(
        SweepiumErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static SweepiumResult<TData> Failure<TData, TSourceData>(ISweepiumResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}