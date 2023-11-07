namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi.DTO;

public record ReevoCommonBoxGameApiResponse<TSuccess>(
    ReevoErrorGameApiResponse? ErrorMessage,
    TSuccess Success);