namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Base;

public interface IParimatchSessionRequest : IParimatchRequest
{
    public string SessionToken { get; init; }
}