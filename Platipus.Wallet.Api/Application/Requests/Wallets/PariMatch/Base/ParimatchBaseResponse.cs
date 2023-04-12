namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch.Base;

public record ParimatchBaseResponse(string TxId,
    string ProcessedTxId,
    DateTimeOffset CreatedAt,
    int Balance,
    bool AlreadyProcessed);
