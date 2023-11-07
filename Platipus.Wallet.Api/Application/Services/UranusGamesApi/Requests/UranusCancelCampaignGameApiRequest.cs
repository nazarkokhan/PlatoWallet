namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.Requests;

using System.Text.Json.Serialization;

public sealed record UranusCancelCampaignGameApiRequest([property: JsonPropertyName("playerCampaignId")] string PlayerCampaignId);