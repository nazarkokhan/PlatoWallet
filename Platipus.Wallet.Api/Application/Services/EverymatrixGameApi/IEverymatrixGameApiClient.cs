namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi;

using External;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;
using Responses;

public interface IEverymatrixGameApiClient
{
    Task<IResult<IHttpClientResult<EverymatrixAwardGameApiResponse, EverymatrixErrorGameApiResponse>>> CreateAwardAsync(
        Uri baseUrl,
        EverymatrixCreateAwardGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<EverymatrixAwardGameApiResponse, EverymatrixErrorGameApiResponse>>> DeleteAwardAsync(
        Uri baseUrl,
        EverymatrixDeleteAwardGameApiRequest request,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<string, EverymatrixErrorGameApiResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        EverymatrixGetLaunchUrlGameApiRequest request,
        CancellationToken cancellationToken = default);
}