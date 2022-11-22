namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;

public record Hub88GameApiCancelRewardRequestDto(
    string? RewardUuid,
    string? OperatorId);