namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

public interface IBetflagBaseRequest
{
    public long Timestamp { get; }

    public string Key { get;}

    public string Hash { get; }
}