namespace Platipus.Wallet.Api.Application.Services.AnakatechGameApi.Requests;

using System.Text.Json.Serialization;

public sealed record AnakatechLaunchGameApiRequest(
    [property: JsonPropertyName("customerId")] string CustomerId,
    [property: JsonPropertyName("brandId")] string BrandId,
    [property: JsonPropertyName("sessionId")] string SessionId,
    [property: JsonPropertyName("securityToken")] string SecurityToken,
    [property: JsonPropertyName("playerId")] string PlayerId,
    [property: JsonPropertyName("providerGameId")] string ProviderGameId,
    [property: JsonPropertyName("playMode")] int PlayMode,
    [property: JsonPropertyName("nickname")] string Nickname,
    [property: JsonPropertyName("balance")] int? Balance,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("country")] string Country,
    [property: JsonPropertyName("lobbyURL")] string LobbyUrl,
    [property: JsonPropertyName("jurisdiction")] string Jurisdiction,
    [property: JsonPropertyName("originUrl")] string OriginUrl,
    [property: JsonPropertyName("realityCheckInterval")] int? RealityCheckInterval,
    [property: JsonPropertyName("realityCheckStartTime")] int? RealityCheckStartTime = null,
    [property: JsonPropertyName("audio")] int? Audio = null,
    [property: JsonPropertyName("minBet")] int? MinBet = null,
    [property: JsonPropertyName("maxTotalBet")] int? MaxTotalBet = null,
    [property: JsonPropertyName("defaultBet")] int? DefaultBet = null);