namespace Platipus.Wallet.Api.Application.Services.MicrogameGameApi;

using Application.Responses.Microgame.Base;
using External;
using Responses;
using Results.HttpClient.WithData;

public interface IMicrogameGameApiClient
{
    Task<IResult<IHttpClientResult<MicrogameLaunchGameApiResponse, MicrogameErrorResponse>>> LaunchAsync(
        Uri baseUrl,
        MicrogameLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}