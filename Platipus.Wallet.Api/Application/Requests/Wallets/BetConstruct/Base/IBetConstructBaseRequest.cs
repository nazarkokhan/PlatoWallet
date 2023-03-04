namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base;

using Requests.Base;

public interface IBetConstructBaseRequest<out TData> : IBaseWalletRequest
where TData : class, IBetConstructDataRequest
{
    public TData Data { get; }
    public DateTime Time { get; }

    public string Hash { get; }
}