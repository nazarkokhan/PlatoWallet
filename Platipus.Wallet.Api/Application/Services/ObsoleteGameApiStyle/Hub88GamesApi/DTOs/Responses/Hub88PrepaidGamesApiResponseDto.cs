namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Responses;

public record Hub88PrepaidGamesApiResponseDto(
    string? PrepaidUuid,
    int? GameId,
    string? GameCode,
    string? Currency,
    int? BetValue,
    int? BetCount);