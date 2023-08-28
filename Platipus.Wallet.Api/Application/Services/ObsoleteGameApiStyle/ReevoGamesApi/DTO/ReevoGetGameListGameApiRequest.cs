namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;

using System.ComponentModel;

public record ReevoGetGameListGameApiRequest(
    [property: DefaultValue("reevo01")] string ApiLogin,
    [property: DefaultValue("reevopass01")] string ApiPassword,
    [property: DefaultValue("USD")] string Currency,
    [property: DefaultValue("getGameList")] string Method = "getGameList");