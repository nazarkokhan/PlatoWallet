using Platipus.Wallet.Api.Application.Results.Base.WithData;

namespace Platipus.Wallet.Api.Application.Results.Sweepium.WithData;

public sealed record SweepiumResult<TData> : BaseResult<SweepiumErrorCode, TData>, ISweepiumResult<TData>
{
    public SweepiumResult(TData data)
        : base(data)
    {
    }

    public SweepiumResult(
        SweepiumErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}