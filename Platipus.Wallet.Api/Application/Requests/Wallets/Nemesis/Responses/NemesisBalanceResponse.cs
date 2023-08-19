namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Responses;

public record NemesisBalanceResponse(
    decimal Balance,
    string Currency);