namespace Platipus.Wallet.Api.Application.Services.MicrogameGameApi.External;

using System.Text.Json.Serialization;

public sealed record MicrogameLaunchGameApiRequest(
    [property: JsonPropertyName("casinoId")] string CasinoId,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("gameId")] string GameId,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("device")] string Device,
    [property: JsonPropertyName("lobbyUrl")] string? LobbyUrl,
    [property: JsonPropertyName("sessionId")] string? SessionId);