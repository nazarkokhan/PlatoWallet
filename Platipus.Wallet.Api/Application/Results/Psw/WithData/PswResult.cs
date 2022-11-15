namespace Platipus.Wallet.Api.Application.Results.Psw.WithData;

using Base.WithData;

public record PswResult<TData> : BaseResult<PswErrorCode, TData>, IPswResult<TData>
{
    public PswResult(TData data)
        : base(data)
    {
    }

    public PswResult(
        PswErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}