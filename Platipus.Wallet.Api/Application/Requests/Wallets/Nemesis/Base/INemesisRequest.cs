namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Base;

using Requests.Base;

public interface INemesisRequest : IBaseWalletRequest
{
    public string SessionToken { get; init; }
}