namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss.Base;

public interface ISoftswissBaseRequest
{
    public string SessionId { get; }

    public string UserId { get; }
}