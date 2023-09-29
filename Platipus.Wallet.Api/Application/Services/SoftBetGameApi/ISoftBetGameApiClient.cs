namespace Platipus.Wallet.Api.Application.Services.SoftBetGameApi;

using External;
using Responses;
using Results.HttpClient.WithData;

public interface ISoftBetGameApiClient
{
    Task<IResult<IHttpClientResult<string, SoftBetGsFailureResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        SoftBetGetLaunchUrlGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}