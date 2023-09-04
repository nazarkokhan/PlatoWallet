namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi.Requests;

using System.Text.Json.Serialization;
using JetBrains.Annotations;

[PublicAPI]
public record EverymatrixCreateAwardGameApiRequest(
    [property: JsonPropertyName("brand")] string Brand,
    string UserId,
    string BonusId,
    string[] GameIds,
    int NumberOfFreeRounds,
    string Currency,
    decimal CoinValue,
    int BetValueLevel,
    DateTime FreeRoundsEndDate);