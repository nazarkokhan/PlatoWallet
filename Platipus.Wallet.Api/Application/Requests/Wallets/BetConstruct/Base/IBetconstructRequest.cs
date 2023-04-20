namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base;

using Requests.Base;

public interface IBetconstructRequest : IBaseWalletRequest
{
    public string Token { get; }
}