namespace Platipus.Wallet.Api.Application.Results.Uranus;

using Base;

public interface IUranusResult : IBaseResult<UranusErrorCode>
{
    public string Balance { get; }
    
    public string Currency { get; }
}