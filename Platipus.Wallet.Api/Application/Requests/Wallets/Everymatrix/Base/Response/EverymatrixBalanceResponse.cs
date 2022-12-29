namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;

public record EverymatrixBalanceResponse(
    int Balance,
    string User,
    string RequestUuid,
    string Currency) : EverymatrixBaseResponse(User, Currency, RequestUuid);