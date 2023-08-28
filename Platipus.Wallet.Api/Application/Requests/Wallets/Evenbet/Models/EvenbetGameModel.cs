namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet.Models;

using System.Text.Json.Serialization;

public sealed record EvenbetGameModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("gameId")] string GameId,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("languages")] string Languages,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("image")] string Image);