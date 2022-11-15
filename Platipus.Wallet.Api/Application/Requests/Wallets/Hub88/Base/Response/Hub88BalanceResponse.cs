namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88.Base.Response;

public record Hub88BalanceResponse(
    int Balance,
    string User,
    string RequestUuid,
    string Currency) : Hub88BaseResponse(User, Currency, RequestUuid);