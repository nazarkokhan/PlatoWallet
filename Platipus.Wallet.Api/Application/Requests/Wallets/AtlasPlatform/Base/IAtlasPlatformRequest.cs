using Platipus.Wallet.Api.Application.Requests.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;

public interface IAtlasPlatformRequest : IBaseWalletRequest
{
    public string? Token { get; }
}