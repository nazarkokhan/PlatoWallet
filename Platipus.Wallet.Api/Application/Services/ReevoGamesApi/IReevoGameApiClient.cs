namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi;

using DTO;
using Results.Reevo.WithData;

public interface IReevoGameApiClient
{
    Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoGetGameGameApiResponse>>> GetGameAsync(
        ReevoGetGameGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoAddFreeRoundsGameApiResponse>>> AddFreeRoundAsync(
        ReevoAddFreeRoundsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoGetGameHistoryGameApiResponse>>> GetGameHistory(
        ReevoGetGameHistoryGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoGetGameListGameApiResponse>>> CreateRewardAsync(
        ReevoGetGameListGameApiRequest request,
        CancellationToken cancellationToken = default);
}