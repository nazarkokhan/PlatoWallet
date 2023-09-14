namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi;

using DTOs.Requests;
using DTOs.Responses;
using Results.HttpClient.WithData;

public interface IHub88GameApiClient
{
    Task<IResult<IHttpClientResult<Hub88GameUrlGameApiResponse, Hub88ErrorGameApiResponse>>> GetLaunchUrlAsync(
        Uri baseUrl,
        string xHub88Signature,
        Hub88GetLaunchUrlGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<Hub88CreateAwardGameApiResponse, Hub88ErrorGameApiResponse>>> CreateAwardAsync(
        Uri baseUrl,
        string xHub88Signature,
        Hub88CreateAwardGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<Hub88DeleteAwardGameApiResponse, Hub88ErrorGameApiResponse>>> DeleteAwardAsync(
        Uri baseUrl,
        string xHub88Signature,
        Hub88DeleteAwardGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<List<Hub88PrepaidGameApiResponseItem>, Hub88ErrorGameApiResponse>>> GetPrepaidsListAsync(
        Uri baseUrl,
        string xHub88Signature,
        Hub88GetPrepaidsListGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<List<Hub88GameGameApiResponseItem>, Hub88ErrorGameApiResponse>>> GetGameListAsync(
        Uri baseUrl,
        string xHub88Signature,
        Hub88GetGameListGameApiRequest gameApiRequest,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<Hub88GetRoundGameApiResponse, Hub88ErrorGameApiResponse>>> GetGameRoundAsync(
        Uri baseUrl,
        string xHub88Signature,
        Hub88GetRoundGameApiRequest request,
        CancellationToken cancellationToken = default);
}