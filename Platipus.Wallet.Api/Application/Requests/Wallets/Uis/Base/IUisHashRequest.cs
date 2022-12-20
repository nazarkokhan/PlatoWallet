namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base;

public interface IUisHashRequest : IUisBaseRequest
{
    public string Hash { get; }

    string GetSource();
}