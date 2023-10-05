namespace Platipus.Wallet.Api.Application.Services.SwGameApi.Requests;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

public sealed record SwGetLaunchUrlGameApiRequest(
    [property: BindProperty(Name = "token")] string Token,
    [property: BindProperty(Name = "key")] string Key,
    [property: JsonPropertyName("userId")] [property: BindProperty(Name = "userid")] string UserId,
    [property: BindProperty(Name = "gameconfig")] string Game,
    [property: BindProperty(Name = "lang")] string Lang);