namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Responses;

public record ParimatchBetWinCancelResponse(
    string TxId,
    string ProcessedTxId,
    DateTimeOffset CreatedAt,
    long Balance,
    bool AlreadyProcessed = false);