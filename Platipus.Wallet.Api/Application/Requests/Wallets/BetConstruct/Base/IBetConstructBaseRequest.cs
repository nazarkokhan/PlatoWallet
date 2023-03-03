namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base;

using Requests.Base;

public interface IBetConstructBaseRequest : IBaseWalletRequest
{
    public DateTime Time { get; }

    public string Hash { get; }
}