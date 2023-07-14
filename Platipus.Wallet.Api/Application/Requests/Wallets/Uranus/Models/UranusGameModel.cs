namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Models;

using System.Text.Json.Serialization;

public sealed record UranusGameModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("vendor")] string Vendor);