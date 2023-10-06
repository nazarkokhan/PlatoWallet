namespace Platipus.Wallet.Api.Application.Services.UisGamesApi;

using Dto;
using Dto.Response;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;

public interface IUisGameApiClient
{
    Task<IResult<IHttpClientResult<UisAwardBonusGameApiResponse, UisGameApiErrorResponse>>> AwardBonusAsync(
        Uri baseUrl,
        UisAwardBonusGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<UisCancelBonusGameApiResponse, UisGameApiErrorResponse>>> CancelBonusAsync(
        Uri baseUrl,
        UisCancelBonusGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<string, UisGameApiErrorResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        UisGetLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}