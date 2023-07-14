namespace Platipus.Wallet.Api.Application.Results.Uranus;

using Platipus.Wallet.Api.Application.Results.Base;

public sealed record UranusResult : BaseResult<UranusErrorCode>, IUranusResult
{
    public UranusResult()
    {
        Balance = "0";
        ErrorDescription = string.Empty;
        Currency = "USD";
    }
    
    public UranusResult(
        UranusErrorCode errorCode, 
        string balance,
        string currency,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        Balance = balance;
        ErrorDescription = description ?? string.Empty;
        Currency = currency;
    }
    
    public string ErrorDescription { get; set; }

    public string? Balance { get; }
    
    public string? Currency { get; }
}