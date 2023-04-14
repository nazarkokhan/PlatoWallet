namespace Platipus.Wallet.Api.Application.Services.UisGamesApi;

using Dto;
using Dto.Response;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;

public interface IUisGameApiClient
{
    Task<IResult<IHttpClientResult<UisGameApiCreateAwardResponse, UisGameApiErrorResponse>>> CreateAwardAsync(
        Uri baseUrl,
        UisCreateAwardGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<UisGameApiDeleteAwardResponse, UisGameApiErrorResponse>>> CancelBonusAsync(
        Uri baseUrl,
        UisDeleteAwardGameApiRequest request,
        CancellationToken cancellationToken = default);
}