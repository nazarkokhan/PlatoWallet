namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;


//TODO ???
public sealed record EmaraPlayResponse(
    IEmaraPlayBaseResponse EmaraPlayBaseResponse, string Error = "0", string Description = "Success");