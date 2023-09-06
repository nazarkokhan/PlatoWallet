namespace Platipus.Wallet.Api.Application.Results.Synot.WithData;

using Base.WithData;

public sealed record SynotResult<TData> : BaseResult<SynotError, TData>, ISynotResult<TData>
{
    public SynotResult(TData data)
        : base(data)
    {
    }

    public SynotResult(
        SynotError error,
        Exception? exception = null)
        : base(error, exception)
    {
    }
}