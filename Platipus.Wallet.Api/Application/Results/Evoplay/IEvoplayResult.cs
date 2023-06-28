namespace Platipus.Wallet.Api.Application.Results.Evoplay;

using Base;

public interface IEvoplayResult : IBaseResult<EvoplayErrorCode>
{
    public string Balance { get; }
    
    public string Currency { get; }
}