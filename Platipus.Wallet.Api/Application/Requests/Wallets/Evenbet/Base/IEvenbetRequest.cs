namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet.Base;

using Requests.Base;

public interface IEvenbetRequest : IBaseWalletRequest
{
    public string Token { get; }
}