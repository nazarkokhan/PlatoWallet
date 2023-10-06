namespace Platipus.Wallet.Api.Application.Services.UisGamesApi.Dto;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

public sealed record UisGetLaunchGameApiRequest(
    [property: BindProperty(Name = "token")] string Token,
    [property: JsonPropertyName("gameConfig")] [property: BindProperty(Name = "gameconfig")] string GameConfig,
    [property: BindProperty(Name = "config")] string Config,
    [property: BindProperty(Name = "room")] string Room,
    [property: BindProperty(Name = "ip")] string Ip,
    [property: BindProperty(Name = "lobby")] string Lobby,
    [property: BindProperty(Name = "lang")] string Lang);