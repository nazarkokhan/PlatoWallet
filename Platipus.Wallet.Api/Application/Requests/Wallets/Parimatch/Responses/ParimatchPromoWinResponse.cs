namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Responses;

public record ParimatchPromoWinResponse(
    string TxId,
    DateTimeOffset CreatedAt,
    long Balance,
    bool AlreadyProcessed = false);