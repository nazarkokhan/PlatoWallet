namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;

using System.ComponentModel;
using System.Text.Json.Serialization;

public record ReevoGetGameGameApiRequest(
    [property: DefaultValue("reevo01")] string ApiLogin,
    [property: DefaultValue("reevopass01")] string ApiPassword,
    [property: DefaultValue("729")] string UserId,
    [property: DefaultValue("reevo_john")] string UserNickname,
    [property: DefaultValue("qwe123")] string UserPassword,
    [property: DefaultValue("en")] string Lang,
    [property: JsonPropertyName("gameid"), DefaultValue("extragems")] string GameId,
    [property: JsonPropertyName("homeurl"), DefaultValue("your_lobby_url")] string HomeUrl,
    [property: DefaultValue("0")] string PlayForFun,
    [property: DefaultValue("USD")] string Currency,
    [property: DefaultValue("reevo_platipus")] string Operator,
    [property: DefaultValue("getGame")] string Method = "getGame");