namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base.Response;

public record SoftBetBalanceResponse(
    int Balance,
    string Currency) : SoftBetBaseResponse;