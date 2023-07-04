namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas.Base;

public interface IAtlasCommonResponse
{
    public string Currency { get; }
    
    public decimal Balance { get; }
    
    public string ClientId { get; }
}