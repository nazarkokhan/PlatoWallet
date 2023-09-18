namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.Models;

using System.Text.Json.Serialization;

public sealed record VegangsterGameModel(
    [property: JsonPropertyName("display_provider")] string DisplayProvider,
    [property: JsonPropertyName("game_code")] string GameCode,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("demo")] bool Demo,
    [property: JsonPropertyName("platforms")] List<List<string>> Platforms);