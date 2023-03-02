namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base.Params;

public record BetParameters(
    int Amount,
    string RoundId,
    string TransactionId);