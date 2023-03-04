namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base.Response;

using System.Text.Json.Serialization;

public record BetConstructGetPlayerInfoResponse(
    bool Result,
    [property: JsonPropertyName("err_desc")] string? ErrDesc,
    [property: JsonPropertyName("err_code")] int? ErrCode,
    string CurrencyId,
    decimal TotalBalance,
    string NickName,
    int UserId,
    int Gender = 1,
    string Country = "Ukraine");