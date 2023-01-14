namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

public record BetflagBaseResponse(
    int Result,
    string Message,
    double Balance,
    bool Bonus,
    string Currency,
    string IdTicket,
    string IdSession,
    string PlayerId,
    string Nickname,
    long Timestamp,
    string Hash);