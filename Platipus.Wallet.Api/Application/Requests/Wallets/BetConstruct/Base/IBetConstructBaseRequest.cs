namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base;

public interface IBetConstructBaseRequest
{
    public DateTime Time { get; }
    public object Data { get; }
    public string Hash { get; }
}