namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi.DTO;

using System.ComponentModel;

public record ReevoGetGameHistoryGameApiRequest(
    string ApiLogin,
    string ApiPassword,
    string RoundId,
    string Currency,
    [DefaultValue("getGameHistory")] string Method = "getGameHistory");