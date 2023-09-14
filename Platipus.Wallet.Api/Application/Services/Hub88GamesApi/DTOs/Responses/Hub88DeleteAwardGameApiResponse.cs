namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record Hub88DeleteAwardGameApiResponse(
    string User,
    string RewardUuid,
    string GameCode,
    int GameId,
    string Currency,
    int BetValue,
    int BetCount);