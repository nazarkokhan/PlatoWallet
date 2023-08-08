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
    [JsonProperty("lobbyURL")] string LobbyUrl,
    [JsonProperty("jurisdiction")] string Jurisdiction,
    [JsonProperty("originUrl")] string OriginUrl,
    [JsonProperty("realityCheckInterval")] int RealityCheckInterval,
    [JsonProperty("realityCheckStartTime")] int? RealityCheckStartTime = null,
    [JsonProperty("audio")] int? Audio = null,
    [JsonProperty("minBet")] int? MinBet = null,
    [JsonProperty("maxTotalBet")] int? MaxTotalBet = null,
    [JsonProperty("defaultBet")] int? DefaultBet = null);