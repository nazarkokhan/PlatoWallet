namespace Platipus.Wallet.Api.Application.Results.Uranus.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public sealed record UranusResult<TData> 
    : BaseResult<UranusErrorCode, TData>, IUranusResult<TData> 
{
    public UranusResult(TData data)
        : base(data)
    {
    }

    public UranusResult(
        UranusErrorCode errorCode,
        string? balance,
        string? currency,
        Exception? exception = null)
        : base(errorCode, exception)
    {
        Balance = balance;
        Currency = currency;
    }
    
    public string? Balance { get; }
    public string? Currency { get; }
    
}