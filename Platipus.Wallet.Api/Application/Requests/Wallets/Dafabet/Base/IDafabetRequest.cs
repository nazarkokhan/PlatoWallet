namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet.Base;

using Requests.Base;

public interface IDafabetRequest : IBaseWalletRequest
{
    public string PlayerId { get; }

    public string Hash { get; }

    string GetSource();
}