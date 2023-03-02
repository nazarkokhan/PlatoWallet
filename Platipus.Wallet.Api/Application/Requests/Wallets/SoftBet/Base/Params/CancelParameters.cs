namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base.Params;

public record CancelParameters(
    int Amount,
    string RoundId,
    string TransactionId);