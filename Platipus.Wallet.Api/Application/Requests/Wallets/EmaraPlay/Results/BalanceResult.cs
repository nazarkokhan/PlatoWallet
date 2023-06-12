namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

public sealed record BalanceResult(
    string Balance, string Currency, string? Bonus = null);