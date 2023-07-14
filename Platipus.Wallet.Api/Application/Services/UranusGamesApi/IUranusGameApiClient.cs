namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi;

using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;

public interface IUranusGameApiClient
{
    Task<IResult<IHttpClientResult<UranusSuccessResponse<UranusGameUrlData>, UranusFailureResponse>>> GetLaunchUrlAsync(
        Uri baseUrl,
        UranusGetLaunchUrlGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<UranusSuccessResponse<UranusAvailableGamesData>, UranusFailureResponse>>> GetAvailableGamesAsync(
        Uri baseUrl,
        UranusGetAvailableGamesGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<UranusSuccessResponse<UranusGameUrlData>, UranusFailureResponse>>> GetDemoLaunchUrlAsync(
        Uri baseUrl,
        UranusGetDemoLaunchUrlGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}