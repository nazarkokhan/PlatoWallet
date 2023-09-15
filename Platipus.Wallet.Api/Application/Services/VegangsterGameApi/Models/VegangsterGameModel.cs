namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.Models;

using System.Text.Json.Serialization;

public sealed record VegangsterGameModel(
    [property: JsonPropertyName("display_provider")] string DisplayProvider,
    [property: JsonPropertyName("game_code")] string GameCode,
    string Name,
    bool Demo,
    List<string> Platforms);