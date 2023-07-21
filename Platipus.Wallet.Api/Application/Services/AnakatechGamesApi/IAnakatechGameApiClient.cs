namespace Platipus.Wallet.Api.Application.Services.AnakatechGamesApi;

using Requests;
using Responses.Anakatech.Base;
using Results.HttpClient.WithData;

public interface IAnakatechGameApiClient
{
    Task<IResult<IHttpClientResult<Stream, AnakatechErrorResponse>>> GetLaunchGameUrlAsBytesAsync(
        Uri baseUrl,
        AnakatechLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}