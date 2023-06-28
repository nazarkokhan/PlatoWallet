namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Base;

using Requests.Base;

public interface IEvoplayRequest : IBaseWalletRequest
{
    public string? Token { get; }
    public string PlayerId { get; }
}