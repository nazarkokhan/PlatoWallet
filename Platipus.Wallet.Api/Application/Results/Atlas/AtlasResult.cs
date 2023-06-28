namespace Platipus.Wallet.Api.Application.Results.Atlas;

using Base;

public sealed record AtlasResult :  BaseResult<AtlasErrorCode>, IAtlasResult
{
    public AtlasResult()
    {
        ErrorDescription = string.Empty;
    }
    
    public AtlasResult(
        AtlasErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }
    
    public string ErrorDescription { get; set; }
    
}