namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas.Base;

public interface IAtlasCommonResponse
{
    public string Currency { get; }
    
    public int Balance { get; }
    
    public string ClientId { get; }
}