using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi.Responses;

namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;

public interface IEmaraPlayGameApiClient
{
    Task<IResult<IHttpClientResult<EmaraPlayGetLauncherUrlResponse, EmaraPlayGameApiErrorResponse>>> GetLauncherUrlAsync(
        Uri baseUrl,
        EmaraPlayGetLauncherUrlRequest urlRequest,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<EmaraPlayGetRoundDetailsResponse, EmaraPlayGameApiErrorResponse>>> GetRoundDetailsAsync(
        Uri baseUrl,
        EmaraPlayGetRoundDetailsRequest urlRequest,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<EmaraPlayAwardResponse, EmaraPlayGameApiErrorResponse>>> GetAwardAsync(
        Uri baseUrl,
        EmaraPlayAwardRequest urlRequest,
        CancellationToken cancellationToken = default);
}