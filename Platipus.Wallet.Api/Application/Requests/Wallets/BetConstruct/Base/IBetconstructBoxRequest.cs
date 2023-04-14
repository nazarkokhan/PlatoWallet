namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base;

using Requests.Base;

public interface IBetconstructBoxRequest<out TRequestData> : IBaseWalletRequest
    where TRequestData : IBetconstructRequest
{
    public string Hash { get; }
    public string Time { get; }
    public TRequestData Data { get; }
}