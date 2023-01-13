namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

public record EmaraPlayBaseResponse(
    string Error,
    string Description,
    Result Result);