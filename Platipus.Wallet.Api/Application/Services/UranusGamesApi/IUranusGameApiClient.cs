namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi;

using Abstaction;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;

public interface IUranusGameApiClient
{
    Task<IResult<IHttpClientResult<UranusSuccessResponse<UranusGameUrlData>, UranusFailureResponse>>> GetGameLaunchUrlAsync(
        Uri baseUrl,
        IUranusCommonGetLaunchUrlApiRequest apiRequest,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<UranusSuccessResponse<UranusAvailableGamesData>, UranusFailureResponse>>>
        GetAvailableGamesAsync(
            Uri baseUrl,
            UranusGetAvailableGamesGameApiRequest apiRequest,
            CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<UranusSuccessResponse<UranusGameUrlData>, UranusFailureResponse>>> GetDemoLaunchUrlAsync(
        Uri baseUrl,
        IUranusCommonGetLaunchUrlApiRequest apiRequest,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<UranusSuccessResponse<string[]>, UranusFailureResponse>>> CreateCampaignAsync(
        Uri baseUrl,
        UranusCreateCampaignGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<UranusSuccessResponse<string[]>, UranusFailureResponse>>> CancelCampaignAsync(
        Uri baseUrl,
        UranusCancelCampaignGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}