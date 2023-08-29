namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGameApi;

using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;
using Responses;

public interface IEmaraPlayGameApiClient
{
    Task<IResult<IHttpClientResult<EmaraPlayGetLauncherUrlResponse, EmaraPlayGameApiErrorResponse>>> GetLauncherUrlAsync(
        Uri baseUrl,
        EmaraplayGetLauncherUrlGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<EmaraPlayGetRoundDetailsResponse, EmaraPlayGameApiErrorResponse>>> GetRoundDetailsAsync(
        Uri baseUrl,
        EmaraplayGetRoundDetailsGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<EmaraPlayAwardResponse, EmaraPlayGameApiErrorResponse>>> GetAwardAsync(
        Uri baseUrl,
        EmaraplayAwardGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<EmaraPlayCancelResponse, EmaraPlayGameApiErrorResponse>>> CancelAsync(
        Uri baseUrl,
        EmaraplayCancelGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}