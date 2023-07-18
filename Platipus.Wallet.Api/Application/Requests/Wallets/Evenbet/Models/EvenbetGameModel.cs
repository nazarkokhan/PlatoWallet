namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet.Models;

using Newtonsoft.Json;

public sealed record EvenbetGameModel(
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("gameId")] string GameId,
    [property: JsonProperty("platform")] string Platform,
    [property: JsonProperty("languages")] string Languages,
    [property: JsonProperty("category")] string Category,
    [property: JsonProperty("image")] string Image);