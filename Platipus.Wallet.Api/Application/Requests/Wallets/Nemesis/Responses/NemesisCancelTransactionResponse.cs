namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisCancelTransactionResponse(
    string ReferenceTransactionId,
    string TransactionId,
    long Balance,
    string Currency,
    decimal BalanceMultiplier) : NemesisBaseResponse;