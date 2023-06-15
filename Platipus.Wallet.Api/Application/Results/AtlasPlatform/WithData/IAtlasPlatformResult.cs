using Platipus.Wallet.Api.Application.Results.Base.WithData;

namespace Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;

public interface IAtlasPlatformResult<out TData> : 
    IBaseResult<AtlasPlatformErrorCode, TData>, IAtlasPlatformResult
{
    
}