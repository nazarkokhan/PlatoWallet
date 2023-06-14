namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

public record EmaraPlayErrorResponse(
    int Error,
    string Description);