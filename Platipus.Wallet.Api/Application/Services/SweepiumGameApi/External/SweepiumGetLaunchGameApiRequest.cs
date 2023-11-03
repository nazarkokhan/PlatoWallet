using System.Text.Json.Serialization;

namespace Platipus.Wallet.Api.Application.Services.SweepiumGameApi.Requests;

public record SweepiumGetLaunchGameApiRequest(
    [property: JsonPropertyName("idGame")] int GameId,
    [property: JsonPropertyName("idCasino")] string CasinoId,
    [property: JsonPropertyName("token")] string? Token,
    [property: JsonPropertyName("idLanguage")] string Language,
    [property: JsonPropertyName("exitUrl")] string ExitUrl,
    [property: JsonPropertyName("forReal")] int ForReal);