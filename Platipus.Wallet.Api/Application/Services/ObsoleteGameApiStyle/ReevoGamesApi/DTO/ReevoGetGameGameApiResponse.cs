namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;

using System.Text.Json.Serialization;

public record ReevoGetGameGameApiResponse(
    string Response,
    [property: JsonPropertyName("gamesession_id")] string GameSessionId,
    [property: JsonPropertyName("sessionid")] string SessionId,
    string Currency,
    int Error);