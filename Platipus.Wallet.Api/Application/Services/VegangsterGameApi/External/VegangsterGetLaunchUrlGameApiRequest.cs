namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.External;

using System.Text.Json.Serialization;

public sealed record VegangsterGetLaunchUrlGameApiRequest(
    string BrandId,
    [property: JsonPropertyName("player_id")] string PlayerId,
    string Token,
    string GameCode,
    string Platform,
    string Currency,
    string Lang,
    string Country,
    string Ip,
    string? LobbyUrl,
    [property: JsonPropertyName("deposit_url")] string? DepositUrl,
    [property: JsonPropertyName("player_nick")] string? PlayerNick) : VegangsterGetLaunchUrlCommonGameApiRequest(
    BrandId,
    GameCode,
    Platform,
    Currency,
    Lang,
    Country,
    Ip,
    LobbyUrl);