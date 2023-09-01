namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Base;

using Platipus.Wallet.Api.Application.Requests.Base;

public interface IParimatchRequest : IBaseWalletRequest
{
    public string SessionToken { get; init; }
}