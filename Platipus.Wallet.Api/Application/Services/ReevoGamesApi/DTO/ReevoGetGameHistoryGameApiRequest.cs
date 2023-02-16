namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi.DTO;

public record ReevoGetGameHistoryGameApiRequest(
    string ApiLogin,
    string ApiPassword,
    string RoundId,
    string Currency,
    string Method = "getGameHistory");