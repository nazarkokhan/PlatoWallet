namespace Platipus.Wallet.Api.Application.Results.Sw.WithData;

using Base.WithData;

public record SwResult<TData> : BaseResult<SwErrorCode, TData>, ISwResult<TData>
{
    public SwResult(TData data)
        : base(data)
    {
    }

    public SwResult(
        SwErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}