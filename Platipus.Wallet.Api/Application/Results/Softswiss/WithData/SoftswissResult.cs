namespace Platipus.Wallet.Api.Application.Results.Softswiss.WithData;

using Base.WithData;

public record SoftswissResult<TData> : BaseResult<SoftswissErrorCode, TData>, ISoftswissResult<TData>
{
    public SoftswissResult(TData data)
        : base(data)
    {
    }

    public SoftswissResult(
        SoftswissErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}