namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.EmaraPlay.Base;

public record EmaraPlayResponse(
    IEmaraplayBaseResponse EmaraplayBaseResponse,
    string Error = "0",
    string Description = "Success");