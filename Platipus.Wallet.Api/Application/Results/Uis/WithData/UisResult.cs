namespace Platipus.Wallet.Api.Application.Results.Uis.WithData;

using Base.WithData;

public record UisResult<TData> : BaseResult<UisErrorCode, TData>, IUisResult<TData>
{
    public UisResult(TData data)
        : base(data)
    {
    }

    public UisResult(
        UisErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}