namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi.External;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

public sealed record EverymatrixGetLaunchUrlGameApiRequest(
    [property: BindProperty(Name = "brand")] string Brand,
    [property: JsonPropertyName("gameCode")] [property: BindProperty(Name = "gamecode")] string GameCode,
    [property: BindProperty(Name = "language")] string Language,
    [property: JsonPropertyName("freePlay")] [property: BindProperty(Name = "freeplay")] bool FreePlay,
    [property: BindProperty(Name = "mobile")] bool Mobile,
    [property: BindProperty(Name = "mode")] string Mode,
    [property: BindProperty(Name = "token")] string Token,
    [property: JsonPropertyName("lobbyUrl")] [property: BindProperty(Name = "lobbyurl")] string LobbyUrl,
    [property: JsonPropertyName("currencyCode")] [property: BindProperty(Name = "currencycode")] string CurrencyCode);