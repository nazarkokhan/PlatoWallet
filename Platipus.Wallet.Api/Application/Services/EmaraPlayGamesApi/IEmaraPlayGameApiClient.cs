using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi.Responses;
using Platipus.Wallet.Api.Application.Services.UisGamesApi.Dto.Response;

namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;

public interface IEmaraPlayGameApiClient
{
    Task<IResult<IHttpClientResult<EmaraPlayGetLauncherUrlResponse, EmaraPlayGameApiErrorResponse>>> GetLauncherUrlAsync(
        Uri baseUrl,
        EmaraPlayGetLauncherUrlRequest urlRequest,
        CancellationToken cancellationToken = default);
}