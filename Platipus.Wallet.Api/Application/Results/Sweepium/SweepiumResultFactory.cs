using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;

namespace Platipus.Wallet.Api.Application.Results.Sweepium;

public static class SweepiumResultFactory
{
    public static SweepiumResult Success() => new();

    public static SweepiumResult<TData> Success<TData>(TData data) => new(data);

    public static SweepiumResult Failure(
        SweepiumErrorCode errorCode,
        int balance,
        string timestamp,
        Exception? exception = null)
    {
        throw new NotImplementedException();
    }

    public static SweepiumResult Failure(ISweepiumResult result)
    {
        throw new NotImplementedException();
    }
         

    public static SweepiumResult<TData> Failure<TData>(
        SweepiumErrorCode errorCode,
        int balance = default,
        string timestamp = "",
        Exception? exception = null)
    {
        throw new NotImplementedException();
    }

    public static SweepiumResult<TData> Failure<TData, TSourceData>(SweepiumResult<TSourceData> result)
    {
        throw new NotImplementedException();
    }
}