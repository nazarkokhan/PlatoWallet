namespace Platipus.Wallet.Api.Application.Results.Everymatrix.WithData;

using Base.WithData;

public record EverymatrixResult<TData> : BaseResult<EverymatrixErrorCode, TData>, IEverymatrixResult<TData>
{
    public EverymatrixResult(TData data)
        : base(data)
    {
    }

    public EverymatrixResult(
        EverymatrixErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}