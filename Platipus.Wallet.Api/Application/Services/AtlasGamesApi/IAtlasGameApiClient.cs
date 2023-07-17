namespace Platipus.Wallet.Api.Application.Services.AtlasGamesApi;

using Application.Requests.Wallets.Atlas.Base;
using Requests;
using Responses.AtlasPlatform;
using Results.HttpClient.WithData;

public interface IAtlasGameApiClient
{
    Task<IResult<IHttpClientResult<AtlasLaunchGameResponse, AtlasErrorResponse>>> LaunchGameAsync(
        Uri baseUrl,
        AtlasGameLaunchGameApiRequest apiRequest,
        string token,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<AtlasGetGamesListResponse, AtlasErrorResponse>>> GetGamesListAsync(
        Uri baseUrl,
        AtlasGetGamesListGameApiRequest apiRequest,
        string token,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<object, AtlasErrorResponse>>> RegisterFreeSpinBonusAsync(
        Uri baseUrl,
        AtlasRegisterFreeSpinBonusGameApiRequest apiRequest,
        string token,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<object, AtlasErrorResponse>>> AssignFreeSpinBonusAsync(
        Uri baseUrl,
        AtlasAssignFreeSpinBonusGameApiRequest apiRequest,
        string token,
        CancellationToken cancellationToken = default);
}