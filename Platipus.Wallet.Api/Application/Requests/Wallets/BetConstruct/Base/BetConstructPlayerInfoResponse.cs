namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base;

public record BetConstructPlayerInfoResponse(
    bool Result,
    string? ErrDesc,
    int? ErrCode,
    string CurrencyId,
    decimal TotalBalance,
    string NickName,
    int Gender,
    string Country,
    string UserID);