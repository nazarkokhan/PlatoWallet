namespace Platipus.Wallet.Api.Application.Services.AnakatechGameApi;

using Responses.Anakatech.Base;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;

public interface IAnakatechGameApiClient
{
    Task<IResult<IHttpClientResult<string, AnakatechErrorResponse>>> GetLaunchGameUrlAsBytesAsync(
        Uri baseUrl,
        AnakatechLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}