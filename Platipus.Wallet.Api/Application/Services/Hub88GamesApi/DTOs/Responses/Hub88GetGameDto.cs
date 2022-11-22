namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Responses;

public record Hub88GetGameDto(
    string UrlThumb,
    string UrlBackground,
    string Product,
    string[] Platforms,
    string Name,
    long GameId,
    string GameCode,
    bool FreebetSupport,
    bool Enabled,
    string Category,
    string[] BlockedCountries);