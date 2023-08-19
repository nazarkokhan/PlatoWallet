namespace Platipus.Wallet.Api.Application.Results.Nemesis.WithData;

using Base.WithData;

public record NemesisResult<TData> : BaseResult<NemesisErrorCode, TData>, INemesisResult<TData>
{
    public NemesisResult(TData data)
        : base(data)
    {
    }

    public NemesisResult(
        NemesisErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}