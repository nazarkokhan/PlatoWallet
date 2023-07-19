namespace Platipus.Wallet.Api.Application.Responses.Evenbet;

using Newtonsoft.Json;

public sealed record EvenbetLoginResponse(
    [property: JsonProperty("token")] string Token,
    [property: JsonProperty("balance")] decimal Balance,
    [property: JsonProperty("currency")] string Currency,
    [property: JsonProperty("nickname")] string Nickname,
    [property: JsonProperty("timestamp")] string Timestamp,
    [property: JsonProperty("userId")] int UserId,
    [property: JsonProperty("country")] string? Country = null);