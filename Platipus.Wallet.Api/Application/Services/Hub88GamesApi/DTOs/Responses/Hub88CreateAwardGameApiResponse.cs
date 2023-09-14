namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record Hub88CreateAwardGameApiResponse(
    string User,
    DateTime StartTime,
    string RewardUuid,
    string PrepaidUuid,
    int GameId,
    string GameCode,
    DateTime EndTime,
    string Currency,
    int BetValue,
    int BetCount);