namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.Hub88GamesApi;

using DTOs.Requests;
using DTOs.Responses;
using Results.Hub88.WithData;

public interface IHub88GamesApiClient
{
    // Task<IResult<IHttpClientResult<Hub88GameApiCreateRewardResponseDto, EverymatrixErrorGameApiResponse>>> CreateAwardAsync(
    //     Uri baseUrl,
    //     Hub88GameApiCreateRewardRequestDto request,
    //     CancellationToken cancellationToken = default);
    //
    // Task<IResult<IHttpClientResult<Hub88GameApiCancelRewardResponseDto, EverymatrixErrorGameApiResponse>>> DeleteAwardAsync(
    //     Uri baseUrl,
    //     Hub88GameApiCancelRewardRequestDto request,
    //     CancellationToken cancellationToken = default);

    //Game
    Task<IHub88Result<Hub88GetLaunchUrlGamesApiResponseDto>> GetLaunchUrlAsync(
        Uri baseUrl,
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

public interface IHub88GameApiClient
{
    // Task<IResult<IHttpClientResult<Hub88GameApiCreateRewardResponseDto, EverymatrixErrorGameApiResponse>>> CreateAwardAsync(
    //     Uri baseUrl,
    //     Hub88GameApiCreateRewardRequestDto request,
    //     CancellationToken cancellationToken = default);
    //
    // Task<IResult<IHttpClientResult<Hub88GameApiCancelRewardResponseDto, EverymatrixErrorGameApiResponse>>> DeleteAwardAsync(
    //     Uri baseUrl,
    //     Hub88GameApiCancelRewardRequestDto request,
    //     CancellationToken cancellationToken = default);

    //Game
    Task<IHub88Result<Hub88GetLaunchUrlGamesApiResponseDto>> GetLaunchUrlAsync(
        Uri baseUrl,
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