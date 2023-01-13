namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

public record EmaraPlayErrorResponse(
    string Error,
    string Description);