namespace Platipus.Wallet.Api.Application.Results.Microgame;

using Base;

public sealed record MicrogameResult : BaseResult<MicrogameStatusCode>, IMicrogameResult
{
    public MicrogameResult()
    {
        ErrorDescription = string.Empty;
    }
    
    public MicrogameResult(
        MicrogameStatusCode statusCode,
        Exception? exception = null,
        string? description = null)
        : base(statusCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }
    
    public string ErrorDescription { get; set; }
}