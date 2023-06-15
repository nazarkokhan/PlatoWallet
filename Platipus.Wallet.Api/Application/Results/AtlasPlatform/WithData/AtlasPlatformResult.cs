using Platipus.Wallet.Api.Application.Results.Base.WithData;

namespace Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;

public sealed record AtlasPlatformResult<TData> : 
    BaseResult<AtlasPlatformErrorCode, TData>, IAtlasPlatformResult<TData>
{
    public AtlasPlatformResult(TData data) 
        : base(data) { }

    public AtlasPlatformResult(
        AtlasPlatformErrorCode errorCode, 
        Exception? exception) : 
        base(errorCode, exception) { }
}