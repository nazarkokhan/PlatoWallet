namespace Platipus.Wallet.Api.Application.Results.Vegangster;

using Base;

public sealed record VegangsterResult : BaseResult<VegangsterResponseStatus>, IVegangsterResult
{
    public VegangsterResult()
    {
        ErrorDescription = string.Empty;
    }
    
    public VegangsterResult(
        VegangsterResponseStatus responseStatus,
        Exception? exception = null,
        string? description = null)
        : base(responseStatus, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }
    
    public string ErrorDescription { get; set; }
}