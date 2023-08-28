namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;

public record ReevoCommonBoxGameApiResponse<TSuccess>(
    ReevoErrorGameApiResponse? ErrorMessage,
    TSuccess Success);