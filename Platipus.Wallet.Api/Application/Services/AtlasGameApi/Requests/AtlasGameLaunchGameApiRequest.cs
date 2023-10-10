namespace Platipus.Wallet.Api.Application.Services.AtlasGameApi.Requests;

using System.Text.Json.Serialization;

public sealed record AtlasGameLaunchGameApiRequest(
    [property: JsonPropertyName("gameId")] string GameId,
    bool Demo,
    [property: JsonPropertyName("isMobile")] bool IsMobile,
    string Token,
    [property: JsonPropertyName("casinoId")] string CasinoId,
    string Language,
    [property: JsonPropertyName("cashierUrl")] string CashierUrl,
    [property: JsonPropertyName("lobbyUrl")] string LobbyUrl);