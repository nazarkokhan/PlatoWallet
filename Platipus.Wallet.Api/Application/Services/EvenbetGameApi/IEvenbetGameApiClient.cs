namespace Platipus.Wallet.Api.Application.Services.EvenbetGameApi;

using Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet.Models;
using Responses.Evenbet.Base;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;

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