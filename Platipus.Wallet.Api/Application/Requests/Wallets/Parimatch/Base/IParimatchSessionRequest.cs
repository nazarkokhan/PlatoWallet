namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Base;

using Requests.Base;

public interface IParimatchSessionRequest : IBaseWalletRequest
{
    public string SessionToken { get; init; }
}