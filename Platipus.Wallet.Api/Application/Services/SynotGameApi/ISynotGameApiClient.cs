namespace Platipus.Wallet.Api.Application.Services.SynotGameApi;

using External;
using Requests;
using Responses.Synot.Base;
using Results.HttpClient.WithData;

public interface ISynotGameApiClient
{
    Task<IResult<IHttpClientResult<SynotGetGamesResponse, SynotErrorResponse>>> GetGamesAsync(
        string casinoId,
        Uri baseUrl,
        CancellationToken cancellationToken = default);
    
    Task<IResult<IHttpClientResult<string, SynotErrorResponse>>> GetGameLaunchScriptAsync(
        string casinoId,
        Uri baseUrl,
        SynotGetGameLaunchScriptGameApiRequest request,
        CancellationToken cancellationToken = default);
}