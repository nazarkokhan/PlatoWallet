using Platipus.Wallet.Api.Application.Requests.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

public interface IEmaraPlayBaseRequest : IBaseWalletRequest
{
    //TODO you can add Provider property here to check it in security filter
    public string? Token { get; }
}