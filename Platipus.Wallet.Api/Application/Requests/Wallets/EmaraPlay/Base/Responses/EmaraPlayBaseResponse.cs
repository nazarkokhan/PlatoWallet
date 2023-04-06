namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.EmaraPlay.Base;

public record EmaraplayBaseResponse(
    string? Currency,
    string? Balance,
    string? Transaction,
    string? Txid) : IEmaraplayBaseResponse;