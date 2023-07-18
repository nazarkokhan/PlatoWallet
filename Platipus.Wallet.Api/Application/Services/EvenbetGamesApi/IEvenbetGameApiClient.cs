namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi;

using External;
using Responses.Evenbet.Base;
using Results.HttpClient.WithData;

public interface IEvenbetGameApiClient
{
    Task<IResult<IHttpClientResult<EvenbetGetGamesResponse, EvenbetFailureResponse>>> GetGamesAsync(
        Uri baseUrl,
        CancellationToken cancellationToken = default);
    
}