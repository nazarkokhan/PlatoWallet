namespace Platipus.Wallet.Api.Application.Services.SwGameApi;

using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;
using Responses;

public interface ISwGameApiClient
{
    Task<IResult<IHttpClientResult<SwAwardGameApiResponse, object>>> CreateFreespin(
        Uri baseUrl,
        SwCreateAwardGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<SwAwardGameApiResponse, object>>> DeleteFreespin(
        Uri baseUrl,
        SwDeleteFreespinGameApiRequest request,
        CancellationToken cancellationToken = default);
}