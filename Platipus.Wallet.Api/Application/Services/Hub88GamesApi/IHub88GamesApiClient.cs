namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi;

using DTOs.Requests;
using DTOs.Responses;
using Results.Hub88.WithData;

public interface IHub88GamesApiClient
{
    //Game
    Task<IHub88Result<Hub88GetLaunchUrlGamesApiResponseDto>> GetLaunchUrlAsync(
        Hub88GetGameLinkGamesApiRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IHub88Result<List<Hub88GetGameDto>>> GetGamesListAsync(
        Hub88GetGamesListRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IHub88Result<Hub88GetRoundGamesApiResponseDto>> GetRoundAsync(
        Hub88GetRoundGamesApiRequestDto request,
        CancellationToken cancellationToken = default);

    //Freebet
    Task<IHub88Result<List<Hub88PrepaidGamesApiResponseDto>>> GetPrepaidsListAsync(
        Hub88PrepaidsListGamesApiRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IHub88Result<Hub88GameApiCreateRewardResponseDto>> CreateRewardAsync(
        Hub88GameApiCreateRewardRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IHub88Result<Hub88GameApiCancelRewardResponseDto>> CancelRewardAsync(
        Hub88GameApiCancelRewardRequestDto request,
        CancellationToken cancellationToken = default);
}