namespace Platipus.Wallet.Api.Application.Services.NemesisGameApi;

using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;
using Responses;

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
        string xIntegrationToken,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<NemesisCurrenciesGameApiResponse[], NemesisErrorGameApiResponse>>> Currencies(
        Uri baseUrl,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<NemesisRoundGameApiResponse, NemesisErrorGameApiResponse>>> Round(
        Uri baseUrl,
        NemesisRoundGameApiRequest request,
        CancellationToken cancellationToken = default);
}