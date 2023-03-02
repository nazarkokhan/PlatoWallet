namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base;

using Requests.Base;

public interface IUisRequest : IBaseWalletRequest
{
    string? Hash { get; }

    string GetSource();
}