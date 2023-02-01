namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

public record BetflagBetWinCancelResponse(
    double Balance,
    string Currency,
    bool IsRetry = false) : BetflagExtendedResponse(Balance, Currency);