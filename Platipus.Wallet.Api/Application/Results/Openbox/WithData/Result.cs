namespace Platipus.Wallet.Api.Application.Results.Openbox.WithData;

using Base.WithData;

public record OpenboxResult<TData> : BaseResult<OpenboxErrorCode, TData>, IOpenboxResult<TData>
{
    public OpenboxResult(TData data) : base(data)
    {
    }

    public OpenboxResult(
        OpenboxErrorCode errorCode,
        Exception? exception = null) : base(errorCode, exception)
    {
    }
}