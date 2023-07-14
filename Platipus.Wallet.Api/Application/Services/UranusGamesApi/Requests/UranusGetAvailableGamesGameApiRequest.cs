namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.Requests;

using Newtonsoft.Json;

public sealed record UranusGetAvailableGamesGameApiRequest([property: JsonProperty("casinoId")] string? CasinoId);