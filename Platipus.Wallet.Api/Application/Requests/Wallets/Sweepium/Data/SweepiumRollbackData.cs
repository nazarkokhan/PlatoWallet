namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Data;

public record SweepiumRollbackData(
    string Token,
    string RoundId,
    string TransactionId,
    string GameId) : SweepiumCommonData(Token, RoundId, TransactionId, GameId);