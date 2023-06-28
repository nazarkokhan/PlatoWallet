namespace Platipus.Wallet.Api.Application.Results.Atlas.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public interface IAtlasResult<out TData> : 
    IBaseResult<AtlasErrorCode, TData>, IAtlasResult
{
    
}