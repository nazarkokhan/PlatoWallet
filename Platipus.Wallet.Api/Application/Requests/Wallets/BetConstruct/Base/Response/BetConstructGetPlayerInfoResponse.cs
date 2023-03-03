namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base.Response;

public record BetConstructGetPlayerInfoResponse(bool Result,
    string? ErrorDescription,
    int? ErrorCode,
    string CurrencyId,
    decimal TotalBalance,
    string NickName,
    int UserId,
    int Gender = 1,
    string Country = "Ukraine");