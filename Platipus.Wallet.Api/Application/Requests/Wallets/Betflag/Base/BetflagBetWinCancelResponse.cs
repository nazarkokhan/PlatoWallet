namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

public record BetflagBetWinCancelResponse(
    int Result,
    string Message,
    double Balance,
    bool Bonus,
    string Currency,
    string IdTicket,
    string IdSession,
    bool IsRetry,
    long Timestamp,
    string Hash);