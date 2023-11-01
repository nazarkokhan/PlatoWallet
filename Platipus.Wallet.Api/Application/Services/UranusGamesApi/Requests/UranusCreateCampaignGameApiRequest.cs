namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.Requests;

using System.Text.Json.Serialization;

public sealed record UranusCreateCampaignGameApiRequest(
    [property: JsonPropertyName("casinoId")] string CasinoId,
    [property: JsonPropertyName("playerId")] string PlayerId,
    [property: JsonPropertyName("amount")] string? Amount,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("campaignEndTime")] string CampaignEndTime,
    [property: JsonPropertyName("spinsCount")] int SpinsCount,
    [property: JsonPropertyName("gameIds")] List<string> GameIds,
    [property: JsonPropertyName("playerCampaignId")] string PlayerCampaignId);