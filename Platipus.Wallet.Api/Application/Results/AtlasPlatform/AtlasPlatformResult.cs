using Platipus.Wallet.Api.Application.Results.Base;

namespace Platipus.Wallet.Api.Application.Results.AtlasPlatform;

public sealed record AtlasPlatformResult :  BaseResult<AtlasPlatformErrorCode>, IAtlasPlatformResult
{
    public AtlasPlatformResult()
    {
        ErrorDescription = string.Empty;
    }
    
    public AtlasPlatformResult(
        AtlasPlatformErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }
    
    public string ErrorDescription { get; set; }
    
}