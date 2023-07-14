namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Base;

using Platipus.Wallet.Api.Application.Requests.Base;

public interface IUranusRequest : IBaseWalletRequest
{
    public string? SessionToken { get; }
    public string PlayerId { get; }
}