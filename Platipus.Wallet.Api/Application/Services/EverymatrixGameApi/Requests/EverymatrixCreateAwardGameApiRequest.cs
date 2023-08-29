namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi.Requests;

using JetBrains.Annotations;

[PublicAPI]
public record EverymatrixCreateAwardGameApiRequest(
    string Brand,
    string UserId,
    string BonusId,
    string[] GameIds,
    int NumberOfFreeRounds,
    string Currency,
    decimal CoinValue,
    int BetValueLevel,
    DateTime FreeRoundsEndDate);