namespace Platipus.Wallet.Api.Application.Requests.Wallets.Anakatech.Base;

using Requests.Base;

public interface IAnakatechRequest : IBaseWalletRequest
{
    public string Secret { get; }

    public string SessionId { get; }

    public string SecurityToken { get; }

    public string PlayerId { get; }

    public string ProviderGameId { get; }

    public int PlayMode { get; }
}