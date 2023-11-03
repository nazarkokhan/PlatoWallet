namespace Platipus.Wallet.Api.Application.Services.SweepiumGameApi.External;

using System.Text.Json.Serialization;

public record SweepiumGetLaunchGameApiRequest(
    [property: JsonPropertyName("idGame")] int GameId,
    [property: JsonPropertyName("idCasino")] string CasinoId,
    [property: JsonPropertyName("token")] string? Token,
    [property: JsonPropertyName("idLanguage")] string Language,
    [property: JsonPropertyName("exitUrl")] string ExitUrl,
    [property: JsonPropertyName("forReal")] int ForReal);