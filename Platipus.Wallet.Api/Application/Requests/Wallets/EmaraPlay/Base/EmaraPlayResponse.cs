namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

public sealed record EmaraPlayResponse(
    IEmaraPlayBaseResponse EmaraPlayBaseResponse, string Error = "0", string Description = "Success");