namespace Platipus.Wallet.Api.Application.Services.ParimatchGameApi;

using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;
using Responses;

public interface IParimatchGameApiClient
{
    Task<IResult<IHttpClientResult<string, ParimatchErrorGameApiResponse>>> LauncherAsync(
        Uri baseUrl,
        ParimatchLauncherGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<ParimatchCreateAwardGameApiResponse, ParimatchErrorGameApiResponse>>> CreateAwardAsync(
        Uri baseUrl,
        ParimatchCreateAwardGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<ParimatchDeleteAwardGameApiResponse, ParimatchErrorGameApiResponse>>> DeleteAwardAsync(
        Uri baseUrl,
        ParimatchDeleteAwardGameApiRequest request,
        CancellationToken cancellationToken = default);
}