namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi;

using External;
using Responses.Vegangster.Base;
using Results.HttpClient.WithData;

public interface IVegangsterGameApiClient
{
    Task<IResult<IHttpClientResult<VegangsterGetLaunchUrlResponse, VegangsterFailureResponse>>> GetLaunchUrlAsync(
        Uri baseUrl,
        string operatorId,
        string xApiSignature,
        IVegangsterCommonGetLaunchUrlApiRequest apiRequest,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<VegangsterGetLaunchUrlResponse, VegangsterFailureResponse>>> GetDemoLaunchUrlAsync(
        Uri baseUrl,
        string operatorId,
        string xApiSignature,
        IVegangsterCommonGetLaunchUrlApiRequest apiRequest,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<VegangsterGetAvailableGamesResponse, VegangsterFailureResponse>>> GetAvailableGamesAsync(
        Uri baseUrl,
        string operatorId,
        string xApiSignature,
        VegangsterGetAvailableGamesGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<VegangsterGrantResponse, VegangsterFailureResponse>>> GrantAsync(
        Uri baseUrl,
        string operatorId,
        string xApiSignature,
        VegangsterGrantGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}