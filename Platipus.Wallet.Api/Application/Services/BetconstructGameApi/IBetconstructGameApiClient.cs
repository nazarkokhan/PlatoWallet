namespace Platipus.Wallet.Api.Application.Services.BetconstructGameApi;

using Application.Requests.Wallets.BetConstruct.Base.Response;
using External;
using Results.HttpClient.WithData;

public interface IBetconstructGameApiClient
{
    Task<IResult<IHttpClientResult<string, BetconstructErrorResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        BetconstructGetLaunchScriptGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}