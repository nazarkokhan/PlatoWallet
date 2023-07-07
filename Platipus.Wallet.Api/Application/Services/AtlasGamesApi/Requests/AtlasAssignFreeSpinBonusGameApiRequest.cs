namespace Platipus.Wallet.Api.Application.Services.AtlasGamesApi.Requests;

using System.Text.Json.Serialization;

public sealed record AtlasAssignFreeSpinBonusGameApiRequest(
    [property: JsonPropertyName("bonusId")]string BonusId,
    [property: JsonPropertyName("bonusInstanceId")]string BonusInstanceId,
    [property: JsonPropertyName("casinoId")]string CasinoId,
    [property: JsonPropertyName("clientId")]string ClientId,
    [property: JsonPropertyName("currency")]string Currency);