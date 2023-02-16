namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi.DTO;

public record ReevoGetGameListGameApiRequest(
    string ApiLogin,
    string ApiPassword,
    string RoundId,
    string Currency,
    string Method = "getGameList");