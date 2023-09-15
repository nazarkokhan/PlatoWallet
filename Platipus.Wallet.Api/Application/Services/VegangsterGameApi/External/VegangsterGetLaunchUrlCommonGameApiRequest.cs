namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.External;

using System.Text.Json.Serialization;

public abstract record VegangsterGetLaunchUrlCommonGameApiRequest(
    [property: JsonPropertyName("brand_id")] string BrandId,
    [property: JsonPropertyName("game_code")] string GameCode,
    string Platform,
    string Currency,
    string Lang,
    string Country,
    string Ip,
    [property: JsonPropertyName("lobby_url")] string? LobbyUrl) : IVegangsterCommonGetLaunchUrlApiRequest;