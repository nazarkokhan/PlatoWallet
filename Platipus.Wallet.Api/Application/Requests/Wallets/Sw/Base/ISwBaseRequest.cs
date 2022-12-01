namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw.Base;

using Requests.Base;

public interface ISwBaseRequest : IBaseWalletRequest
{
    public int ProviderId { get; }

    public int UserId { get; }

    public Guid Token { get; }
}