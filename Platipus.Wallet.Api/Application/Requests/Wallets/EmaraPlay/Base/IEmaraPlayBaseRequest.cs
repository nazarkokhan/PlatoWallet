using Platipus.Wallet.Api.Application.Requests.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

public interface IEmaraPlayBaseRequest : IBaseWalletRequest
{
    public string? Token { get; }
}