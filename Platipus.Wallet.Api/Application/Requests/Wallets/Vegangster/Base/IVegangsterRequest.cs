namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster.Base;

using Requests.Base;

public interface IVegangsterRequest : IBaseWalletRequest
{
    public string Token { get; }
    
    public string PlayerId { get; }
}