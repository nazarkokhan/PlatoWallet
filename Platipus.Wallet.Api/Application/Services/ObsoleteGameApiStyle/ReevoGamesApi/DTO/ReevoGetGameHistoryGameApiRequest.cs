namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;

using System.ComponentModel;

public record ReevoGetGameHistoryGameApiRequest(
    [property: DefaultValue("reevo01")] string ApiLogin,
    [property: DefaultValue("reevopass01")] string ApiPassword,
    [property: DefaultValue("173434923")] string RoundId,
    [property: DefaultValue("USD")] string Currency,
    [property: DefaultValue("getGameHistory")] string Method = "getGameHistory");