namespace Platipus.Wallet.Api.Application.Services.NemesisGamesApi;

using Requests;
using Responses;
using Results.HttpClient.WithData;

public interface INemesisGameApiClient
{
    Task<IResult<IHttpClientResult<string, NemesisErrorGameApiResponse>>> LauncherAsync(
        Uri baseUrl,
        NemesisLauncherGameApiRequest request,
        string xIntegrationToken,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<NemesisCreateAwardGameApiResponse, NemesisErrorGameApiResponse>>> CreateAwardAsync(
        Uri baseUrl,
        NemesisCreateAwardGameApiRequest request,
        string xIntegrationToken,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<NemesisCancelAwardGameApiResponse, NemesisErrorGameApiResponse>>> CancelAwardAsync(
        Uri baseUrl,
        NemesisCancelAwardGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<NemesisCurrenciesGameApiResponse[], NemesisErrorGameApiResponse>>> Currencies(
        Uri baseUrl,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<NemesisRoundGameApiResponse, NemesisErrorGameApiResponse>>> Round(
        Uri baseUrl,
        NemesisRoundGameApiRequest request,
        CancellationToken cancellationToken = default);
}