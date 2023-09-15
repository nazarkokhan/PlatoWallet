namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.External;

public sealed record VegangsterGetDemoLaunchUrlGameApiRequest(
    string BrandId,
    string GameCode,
    string Platform,
    string Currency,
    string Lang,
    string Country,
    string Ip,
    string? LobbyUrl) : VegangsterGetLaunchUrlCommonGameApiRequest(
    BrandId,
    GameCode,
    Platform,
    Currency,
    Lang,
    Country,
    Ip,
    LobbyUrl);