namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.Requests;

using Abstaction;
using Newtonsoft.Json;

public sealed record UranusGetLaunchUrlGameApiRequest(
    [property: JsonProperty("gameId")] string GameId,
    [property: JsonProperty("sessionToken")] string SessionToken,
    [property: JsonProperty("language")] string Language,
    [property: JsonProperty("currency")] string Currency,
    [property: JsonProperty("playerId")] string PlayerId,
    [property: JsonProperty("casinoId")] string CasinoId = "",
    [property: JsonProperty("platformType")] string PlatformType = "",
    [property: JsonProperty("countryCode")] string CountryCode = "",
    [property: JsonProperty("depositUrl")] string DepositUrl = "",
    [property: JsonProperty("lobbyUrl")] string LobbyUrl = "",
    [property: JsonProperty("playerIp")] string PlayerIp = "") : IUranusCommonGetLaunchUrlApiRequest;