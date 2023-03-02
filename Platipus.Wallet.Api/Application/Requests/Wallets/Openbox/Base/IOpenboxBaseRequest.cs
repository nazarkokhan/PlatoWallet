namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox.Base;

using Requests.Base;

public interface IOpenboxBaseRequest : IBaseWalletRequest
{
    public string Token { get; }
}