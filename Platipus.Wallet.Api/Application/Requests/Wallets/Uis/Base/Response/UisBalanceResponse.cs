namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base.Response;

public record UisBalanceResponse(
    string PlayerId,
    string Currency,
    decimal Balance) : UisBaseResponse;