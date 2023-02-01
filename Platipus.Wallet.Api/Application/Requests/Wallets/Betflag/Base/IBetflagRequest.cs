namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

public interface IBetflagRequest : IBetflagBaseRequest
{
    public string Key { get; }
}