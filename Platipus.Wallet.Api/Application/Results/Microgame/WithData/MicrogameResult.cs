namespace Platipus.Wallet.Api.Application.Results.Microgame.WithData;

using Base.WithData;

public sealed record MicrogameResult<TData> : BaseResult<MicrogameStatusCode, TData>, IMicrogameResult<TData>
{
    public MicrogameResult(TData data)
        : base(data)
    {
    }

    public MicrogameResult(MicrogameStatusCode errorCode, Exception? exception)
        : base(errorCode, exception)
    {
    }
}