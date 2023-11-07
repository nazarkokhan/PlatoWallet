using Microsoft.AspNetCore.Mvc;

namespace Platipus.Wallet.Api.Application.Services.SweepiumGameApi.External;

using System.Text.Json.Serialization;

public record SweepiumGetLaunchGameApiRequest(
    [property: JsonPropertyName("idGame")] [property: BindProperty(Name = "idGame")] string GameId,
    [property: JsonPropertyName("idCasino")] [property: BindProperty(Name = "idCasino")] string CasinoId,
    [property: JsonPropertyName("token")] [property: BindProperty(Name = "token")] string? Token,
    [property: JsonPropertyName("idLanguage")] [property: BindProperty(Name = "idLanguage")] string Language,
    [property: JsonPropertyName("exitUrl")] [property: BindProperty(Name = "exitUrl")] string ExitUrl,
    [property: JsonPropertyName("forReal")] [property: BindProperty(Name = "forReal")] int ForReal);