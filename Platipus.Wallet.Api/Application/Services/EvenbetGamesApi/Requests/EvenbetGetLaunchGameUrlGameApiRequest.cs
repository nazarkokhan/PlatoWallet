namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi.Requests;

using Newtonsoft.Json;

public sealed record EvenbetGetLaunchGameUrlGameApiRequest(
    [property: JsonProperty("gameId")] string GameId,
    [property: JsonProperty("mode")] bool Mode,
    [property: JsonProperty("casinoId")] string CasinoId,
    [property: JsonProperty("language")] string Language,
    [property: JsonProperty("platform")] string Platform,
    [property: JsonProperty("currency")] string Currency = "",
    [property: JsonProperty("token")] string Token = "");