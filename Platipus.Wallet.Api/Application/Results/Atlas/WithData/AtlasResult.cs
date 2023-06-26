namespace Platipus.Wallet.Api.Application.Results.Atlas.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public sealed record AtlasResult<TData> : 
    BaseResult<AtlasErrorCode, TData>, IAtlasResult<TData>
{
    public AtlasResult(TData data) 
        : base(data) { }

    public AtlasResult(
        AtlasErrorCode errorCode, 
        Exception? exception) : 
        base(errorCode, exception) { }
}