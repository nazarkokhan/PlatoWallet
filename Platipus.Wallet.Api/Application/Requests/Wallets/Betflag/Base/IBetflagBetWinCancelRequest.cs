namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

public interface IBetflagBetWinCancelRequest : IBetflagRequest
{
    public string TransactionId { get; }
}