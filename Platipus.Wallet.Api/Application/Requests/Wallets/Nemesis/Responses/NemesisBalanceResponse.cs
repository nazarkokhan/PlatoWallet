namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisBalanceResponse(
    long Balance,
    string Currency,
    decimal BalanceMultiplier);