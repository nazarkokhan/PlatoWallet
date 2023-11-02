using Platipus.Wallet.Api.Application.Requests.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;

public interface ISweepiumBoxRequest<out TRequestData> : IBaseWalletRequest
    where TRequestData : ISweepiumRequest
{
    public string Hash { get; }
    public string Time { get; }
    public TRequestData Data { get; }
}