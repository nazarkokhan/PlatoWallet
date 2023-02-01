namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

public record BetflagBalanceResponse(
    double Balance,
    string Currency,
    string PlayerId,
    string Nickname) : BetflagExtendedResponse(Balance, Currency);