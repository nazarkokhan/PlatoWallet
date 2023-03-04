namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base.Response;

using System.Text.Json.Serialization;

public record class BetConstructBaseResponse(
    bool Result,
    [property: JsonPropertyName("err_desc")] string? ErrDesc,
    [property: JsonPropertyName("err_code")] int? ErrCode,
    long TransactionId,
    decimal Balance);