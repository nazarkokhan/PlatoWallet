namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi.DTO;

using System.ComponentModel;

public record ReevoGetGameListGameApiRequest(
    string ApiLogin,
    string ApiPassword,
    string RoundId,
    string Currency,
    [DefaultValue("getGameList")] string Method = "getGameList");