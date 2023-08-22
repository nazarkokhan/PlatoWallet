namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Responses;

public record NemesisBalanceResponse(
    long Balance,
    string Currency,
    decimal BalanceMultiplier);