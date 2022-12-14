namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal.Base.Response;

public record GamesGlobalMetaDto();

public record GamesGlobalBalanceResponse(
    int Balance,
    string User,
    string RequestUuid,
    string Currency) : GamesGlobalBaseResponse(User, Currency, RequestUuid);