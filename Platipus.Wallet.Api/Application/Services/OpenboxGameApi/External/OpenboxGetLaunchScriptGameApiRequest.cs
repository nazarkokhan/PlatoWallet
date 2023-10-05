namespace Platipus.Wallet.Api.Application.Services.OpenboxGameApi.External;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

public sealed record OpenboxGetLaunchScriptGameApiRequest(
    [property: BindProperty(Name = "token")] string Token,
    [property: JsonPropertyName("agencyUId")] [property: BindProperty(Name = "agency-uid")] string AgencyUId,
    [property: JsonPropertyName("playerUId")] [property: BindProperty(Name = "player-uid")] string PlayerUId,
    [property: JsonPropertyName("playerId")] [property: BindProperty(Name = "player-id")] string PlayerId,
    [property: JsonPropertyName("gameId")] [property: BindProperty(Name = "game-id")] string GameId,
    [property: BindProperty(Name = "currency")] string Currency,
    [property: JsonPropertyName("playerType")] [property: BindProperty(Name = "player-type")] int PlayerType,
    [property: BindProperty(Name = "country")] string Country,
    [property: BindProperty(Name = "language")] string Language,
    [property: JsonPropertyName("backUrl")] [property: BindProperty(Name = "backurl")] string BackUrl);