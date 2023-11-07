namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi;

using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using DTO;

public interface IReevoGameApiClient
{
    Task<IResult<IHttpClientResult<ReevoGetGameGameApiResponse, ReevoErrorGameApiResponse>>> GetGameAsync(
        Uri baseUrl,
        ReevoGetGameGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<ReevoAddFreeRoundsGameApiResponse, ReevoErrorGameApiResponse>>> AddFreeRoundsAsync(
        Uri baseUrl,
        ReevoAddFreeRoundsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<ReevoErrorGameApiResponse, ReevoErrorGameApiResponse>>> RemoveFreeRoundsAsync(
        Uri baseUrl,
        ReevoRemoveFreeRoundsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<ReevoGetGameHistoryGameApiResponse, ReevoErrorGameApiResponse>>> GetGameHistoryAsync(
        Uri baseUrl,
        ReevoGetGameHistoryGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<ReevoGetGameListGameApiResponse, ReevoErrorGameApiResponse>>> GetGameListAsync(
        Uri baseUrl,
        ReevoGetGameListGameApiRequest request,
        CancellationToken cancellationToken = default);
}