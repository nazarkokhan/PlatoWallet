namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw.Base;

using Requests.Base;

public interface IPswBaseRequest : IBaseWalletRequest
{
    public Guid SessionId { get; }

    public string User { get; }
}