namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;

public record Hub88GetRoundGamesApiRequestDto(
    string User,
    string TransactionUuid,
    string Round,
    string OperatorId);

public record Hub88PrepaidsListGamesApiRequestDto(
    string OperatorId,
    int GameId,
    string GameCode);

public record Hub88PrepaidGamesApiResponseDto(
    string PrepaidUuid,
    int? GameId,
    string GameCode,
    string Currency,
    int? BetValue,
    int? BetCount);

public record Hub88GameApiCreateRewardRequestDto(
    string RewardUuid,
    string Currency,
    string User,
    string SubPartnerId,
    DateTime? StartTime,
    string PrepaidUuid,
    int? OperatorId,
    int? GameId,
    string GameCode,
    DateTime? EndTime,
    int? BetValue,
    int? BetCount);

public record Hub88GameApiCreateRewardResponseDto(
    string User,
    DateTime? StartTime,
    string RewardUuid,
    string PrepaidUuid,
    int? GameId,
    string GameCode,
    DateTime? EndTime,
    string Currency,
    int? BetValue,
    int? BetCount);

public record Hub88GameApiCancelRewardRequestDto(
    string RewardUuid,
    int? OperatorId);

public record Hub88GameApiCancelRewardResponseDto(
    string User,
    DateTime? StartTime,
    string RewardUuid,
    string PrepaidUuid,
    int? GameId,
    string GameCode,
    DateTime? EndTime,
    string Currency,
    int? BetValue,
    int? BetCount);