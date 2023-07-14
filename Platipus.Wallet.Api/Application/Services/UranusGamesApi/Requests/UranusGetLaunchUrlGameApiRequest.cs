namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.Requests;

using System.Text.Json.Serialization;

public sealed record UranusGetLaunchUrlGameApiRequest(
    [property: JsonPropertyName("gameId")] string GameId,
    [property: JsonPropertyName("sessionToken")] string SessionToken,
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("playerId")] string PlayerId,
    [property: JsonPropertyName("platformType")] string PlatformType,
    [property: JsonPropertyName("countryCode")] string CountryCode,
    [property: JsonPropertyName("depositUrl")] string DepositUrl,
    [property: JsonPropertyName("lobbyUrl")] string LobbyUrl,
    [property: JsonPropertyName("playerIp")] string PlayerIp);