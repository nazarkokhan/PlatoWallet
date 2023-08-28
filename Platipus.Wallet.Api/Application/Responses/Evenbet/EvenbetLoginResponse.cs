namespace Platipus.Wallet.Api.Application.Responses.Evenbet;

using System.Text.Json.Serialization;

public sealed record EvenbetLoginResponse(
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("balance")] int Balance,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("nickname")] string Nickname,
    [property: JsonPropertyName("timestamp")] string Timestamp,
    [property: JsonPropertyName("userId")] int UserId,
    [property: JsonPropertyName("country")] string? Country = null);