namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Responses;

public record Hub88GameApiCreateRewardResponseDto(
    string? User,
    DateTime? StartTime,
    string? RewardUuid,
    string? PrepaidUuid,
    int? GameId,
    string? GameCode,
    DateTime? EndTime,
    string? Currency,
    int? BetValue,
    int? BetCount);