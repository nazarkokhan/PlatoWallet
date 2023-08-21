namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Responses;

public record NemesisBetWinRollbackResponse(
    string ProviderTransactionId,
    string TransactionId,
    decimal Balance,
    string Currency)
{
    public bool Duplicated { get; init; } = false;
    public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}