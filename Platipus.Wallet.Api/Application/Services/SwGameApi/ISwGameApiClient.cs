namespace Platipus.Wallet.Api.Application.Services.SwGameApi;

using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;

public interface ISwGameApiClient
{
    Task<IResult<IHttpClientResult<SwAwardGameApiResponse, SwErrorGameApiResponse>>> CreateFreespin(
        Uri baseUrl,
        SwCreateAwardGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<SwAwardGameApiResponse, SwErrorGameApiResponse>>> DeleteFreespin(
        Uri baseUrl,
        SwDeleteFreespinGameApiRequest request,
        CancellationToken cancellationToken = default);
}