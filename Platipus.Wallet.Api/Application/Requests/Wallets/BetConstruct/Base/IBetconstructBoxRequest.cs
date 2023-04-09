namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base;

public interface IBetconstructBoxRequest<out TRequestData>
    where TRequestData : IBetconstructRequest
{
    public string Hash { get; }
    public string Time { get; }
    public TRequestData Data { get; }
}