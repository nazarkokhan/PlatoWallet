namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base.Response;

using System.Text.Json.Serialization;

public record BetconstructBaseResponse(
    bool Result = true,
    [property: JsonPropertyName("err_desc")] string ErrDesc = null!,
    [property: JsonPropertyName("err_code")] int ErrCode = 0);