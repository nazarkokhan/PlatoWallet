namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base;

public interface IUisHashRequest : IUisBaseRequest
{
    // string UserId { get; set; }

    string? Hash { get; }

    string GetSource();
}