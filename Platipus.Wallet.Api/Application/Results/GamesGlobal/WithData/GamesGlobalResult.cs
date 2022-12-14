namespace Platipus.Wallet.Api.Application.Results.GamesGlobal.WithData;

using Base.WithData;

public record GamesGlobalResult<TData> : BaseResult<GamesGlobalErrorCode, TData>, IGamesGlobalResult<TData>
{
    public GamesGlobalResult(TData data)
        : base(data)
    {
    }

    public GamesGlobalResult(
        GamesGlobalErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}