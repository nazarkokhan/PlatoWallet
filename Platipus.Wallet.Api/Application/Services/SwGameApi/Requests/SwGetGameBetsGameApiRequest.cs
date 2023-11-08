namespace Platipus.Wallet.Api.Application.Services.SwGameApi.Requests;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

public sealed record SwGetGameBetsGameApiRequest(
    [property: JsonPropertyName("key")] [property: BindProperty(Name = "key")] string Key,
    [property: JsonPropertyName("games")] [property: BindProperty(Name = "games")] string Games,
    [property: JsonPropertyName("launchId")] [property: BindProperty(Name = "launchid")] string LaunchId);