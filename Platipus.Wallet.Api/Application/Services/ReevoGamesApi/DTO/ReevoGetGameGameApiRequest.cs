namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi.DTO;

using System.Text.Json.Serialization;

public record ReevoGetGameGameApiRequest(
    string ApiLogin,
    string ApiPassword,
    string UserId,
    string UserNickname,
    string UserPassword,
    string Lang,
    [property: JsonPropertyName("gameid")] string GameId,
    [property: JsonPropertyName("homeurl")] string HomeUrl,
    string PlayForFun,
    string Currency,
    string Operator,
    string Method = "getGame");