namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

using Requests.Base;

public interface IBetflagBaseRequest : IBaseWalletRequest
{
    public long Timestamp { get; }

    public string Hash { get; }

    public string ApiName { get; }
}