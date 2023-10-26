namespace Platipus.Wallet.Api.Application.Services.MicrogameGameApi;

using Requests;
using Responses.Microgame.Base;
using Results.HttpClient.WithData;

public interface IMicrogameGameApiClient
{
    Task<IResult<IHttpClientResult<string, MicrogameErrorResponse>>> LaunchAsync(
        Uri baseUrl,
        MicrogameLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}