namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas.Base;

using Platipus.Wallet.Api.Application.Requests.Base;

public interface IAtlasRequest : IBaseWalletRequest
{
    public string? Token { get; }
}