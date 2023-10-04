namespace Platipus.Wallet.Api.Application.Services.BetconstructGameApi.External;

using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

[PublicAPI]
public sealed record BetconstructGetLaunchScriptGameApiRequest(
    [property: JsonPropertyName("gameId")] [property: BindProperty(Name = "gameID")] int GameId,
    [property: BindProperty(Name = "language")] string Language,
    [property: BindProperty(Name = "mode")] string Mode,
    [property: BindProperty(Name = "token")] string? Token,
    [property: BindProperty(Name = "partner")] string Partner);