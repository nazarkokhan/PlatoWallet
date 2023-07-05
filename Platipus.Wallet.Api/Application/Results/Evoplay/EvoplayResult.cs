namespace Platipus.Wallet.Api.Application.Results.Evoplay;

using Base;

public sealed record EvoplayResult : BaseResult<EvoplayErrorCode>, IEvoplayResult
{
    public EvoplayResult()
    {
        Balance = "0";
        ErrorDescription = string.Empty;
        Currency = "USD";
    }
    
    public EvoplayResult(
        EvoplayErrorCode errorCode, 
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