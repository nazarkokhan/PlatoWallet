namespace Platipus.Wallet.Api.Application.Services.AtlasGamesApi.Requests;

using System.Text.Json.Serialization;

public sealed record AtlasRegisterFreeSpinBonusGameApiRequest(
    [property: JsonPropertyName("gameId")]string GameId,
    [property: JsonPropertyName("bonusId")]string BonusId,
    [property: JsonPropertyName("casinoId")]string? CasinoId,
    [property: JsonPropertyName("spinsCount")]int SpinsCount,
    [property: JsonPropertyName("betValues")]List<Dictionary<string, int>> BetValues,
    [property: JsonPropertyName("startDate")]string StartDate,
    [property: JsonPropertyName("expirationDate")]string ExpirationDate);