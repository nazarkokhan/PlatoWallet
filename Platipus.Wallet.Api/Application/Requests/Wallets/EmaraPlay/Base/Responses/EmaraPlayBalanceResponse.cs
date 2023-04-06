namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.EmaraPlay.Base;

public record EmaraPlayBalanceResponse(string? Currency,
    string? Balance) : IEmaraplayBaseResponse;