namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisBetWinResponse(
    string ProviderTransactionId,
    string TransactionId,
    long Balance,
    string Currency,
    decimal BalanceMultiplier) : NemesisBaseResponse;