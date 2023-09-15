namespace Platipus.Wallet.Api.Application.Results.Vegangster.WithData;

using Base.WithData;

public sealed record VegangsterResult<TData> : BaseResult<VegangsterResponseStatus, TData>, IVegangsterResult<TData>
{
    public VegangsterResult(TData data)
        : base(data)
    {
    }

    public VegangsterResult(VegangsterResponseStatus errorCode, Exception? exception)
        : base(errorCode, exception)
    {
    }
}