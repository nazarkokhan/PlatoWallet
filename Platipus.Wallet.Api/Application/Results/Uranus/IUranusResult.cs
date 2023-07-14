namespace Platipus.Wallet.Api.Application.Results.Uranus;

using Platipus.Wallet.Api.Application.Results.Base;

public interface IUranusResult : IBaseResult<UranusErrorCode>
{
    public string Balance { get; }
    
    public string Currency { get; }
}