namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi;

using DTO;
using Results.Reevo.WithData;

public interface IReevoGameApiClient
{
    Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoGetGameGameApiResponse>>> GetGameAsync(
        Uri baseUrl,
        ReevoGetGameGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoAddFreeRoundsGameApiResponse>>> AddFreeRoundAsync(
        Uri? baseUrl,
        ReevoAddFreeRoundsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoGetGameHistoryGameApiResponse>>> GetGameHistory(
        Uri? baseUrl,
        ReevoGetGameHistoryGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoGetGameListGameApiResponse>>> CreateRewardAsync(
        Uri? baseUrl,
        ReevoGetGameListGameApiRequest request,
        CancellationToken cancellationToken = default);
}