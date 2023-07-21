namespace Platipus.Wallet.Api.Application.Services.AnakatechGamesApi.Requests;

using Newtonsoft.Json;

public sealed record AnakatechLaunchGameApiRequest(
    [JsonProperty("customerId")] string CustomerId,
    [JsonProperty("brandId")] string BrandId,
    [JsonProperty("sessionId")] string SessionId,
    [JsonProperty("securityToken")] string SecurityToken,
    [JsonProperty("playerId")] string PlayerId,
    [JsonProperty("providerGameId")] string ProviderGameId,
    [JsonProperty("playMode")] int PlayMode,
    [JsonProperty("nickname")] string Nickname,
    [JsonProperty("balance")] int Balance,
    [JsonProperty("currency")] string Currency,
    [JsonProperty("language")] string Language,
    [JsonProperty("country")] string Country,
    [JsonProperty("lobbyUrl")] string LobbyUrl,
    [JsonProperty("jurisdiction")] string Jurisdiction,
    [JsonProperty("originUrl")] string OriginUrl,
    [JsonProperty("realityCheckInterval")] int RealityCheckInterval,
    [JsonProperty("realityCheckStartTime")] int? RealityCheckStartTime,
    [JsonProperty("audio")] int? Audio,
    [JsonProperty("minBet")] int? MinBet,
    [JsonProperty("maxTotalBet")] int? MaxTotalBet,
    [JsonProperty("defaultBet")] int? DefaultBet);