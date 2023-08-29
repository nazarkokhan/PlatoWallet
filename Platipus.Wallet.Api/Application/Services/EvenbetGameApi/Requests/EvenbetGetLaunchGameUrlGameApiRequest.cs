namespace Platipus.Wallet.Api.Application.Services.EvenbetGameApi.Requests;

using System.Text.Json.Serialization;

public sealed record EvenbetGetLaunchGameUrlGameApiRequest(
    [property: JsonPropertyName("gameId")] string GameId,
    [property: JsonPropertyName("mode")] bool Mode,
    [property: JsonPropertyName("casinoId")] string CasinoId,
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("currency")] string Currency = "",
    [property: JsonPropertyName("token")] string Token = "");