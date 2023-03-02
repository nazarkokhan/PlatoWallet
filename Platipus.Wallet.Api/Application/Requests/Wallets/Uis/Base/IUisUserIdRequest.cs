namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base;

public interface IUisUserIdRequest : IUisRequest
{
    string UserId { get; set; }
}