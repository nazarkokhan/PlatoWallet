namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.Requests;

using System.Text.Json.Serialization;

public sealed record UranusGetAvailableGamesGameApiRequest([property: JsonPropertyName("casinoId")] string? CasinoId);