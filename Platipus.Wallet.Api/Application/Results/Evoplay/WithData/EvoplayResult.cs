namespace Platipus.Wallet.Api.Application.Results.Evoplay.WithData;

using Base.WithData;

public sealed record EvoplayResult<TData> 
    : BaseResult<EvoplayErrorCode, TData>, IEvoplayResult<TData> 
{
    public EvoplayResult(TData data)
        : base(data)
    {
    }

    public EvoplayResult(
        EvoplayErrorCode errorCode,
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