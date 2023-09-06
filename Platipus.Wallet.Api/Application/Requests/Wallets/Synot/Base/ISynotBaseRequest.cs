namespace Platipus.Wallet.Api.Application.Requests.Wallets.Synot.Base;

using Requests.Base;

public interface ISynotBaseRequest : IBaseWalletRequest
{
    public string? Token { get; }
}