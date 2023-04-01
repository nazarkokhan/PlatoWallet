namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss.Base;

using Requests.Base;

public interface ISoftswissBaseRequest : IBaseWalletRequest
{
    public string SessionId { get; }

    public string UserId { get; }
}