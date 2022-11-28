namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet.Base;

using Requests.Base;

public interface IDatabetBaseRequest : IBaseWalletRequest, ISourceRequest
{
    public string PlayerId { get; }

    public string Hash { get; }
}