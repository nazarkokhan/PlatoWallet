namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi;

using DTO;
using Results.HttpClient.WithData;

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