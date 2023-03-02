namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base.Params;

public record WinParameters(
    int Amount,
    string RoundId,
    string TransactionId,
    bool CloseRound);