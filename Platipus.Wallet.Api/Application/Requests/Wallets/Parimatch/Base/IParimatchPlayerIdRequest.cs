namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Base;

public interface IParimatchPlayerIdRequest : IParimatchRequest
{
    public string PlayerId { get; init; }
}