namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi;

using Application.Requests.Wallets.Evenbet.Models;
using External;
using Requests;
using Responses.Evenbet.Base;
using Results.HttpClient.WithData;

public interface IEvenbetGameApiClient
{
    Task<IResult<IHttpClientResult<List<EvenbetGameModel>, EvenbetFailureResponse>>> GetGamesAsync(
        Uri baseUrl,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<string, EvenbetFailureResponse>>> GetGameLaunchUrlAsync(
        Uri baseUrl,
        EvenbetGetLaunchGameUrlGameApiRequest request,
        CancellationToken cancellationToken = default);
    
}