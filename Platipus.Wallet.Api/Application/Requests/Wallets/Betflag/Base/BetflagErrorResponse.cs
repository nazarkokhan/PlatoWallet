namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

public record BetflagErrorResponse(
    int Result,
    string Message,
    long TimeStamp,
    string Hash);