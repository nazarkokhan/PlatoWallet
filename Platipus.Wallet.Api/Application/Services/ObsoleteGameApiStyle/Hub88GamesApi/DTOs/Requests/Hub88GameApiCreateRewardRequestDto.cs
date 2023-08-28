namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Requests;

public record Hub88GameApiCreateRewardRequestDto(
    string? RewardUuid,
    string? Currency,
    string? User,
    string? SubPartnerId,
    DateTime? StartTime,
    string? PrepaidUuid,
    string? OperatorId,
    int? GameId,
    string? GameCode,
    DateTime? EndTime,
    int? BetValue,
    int? BetCount);