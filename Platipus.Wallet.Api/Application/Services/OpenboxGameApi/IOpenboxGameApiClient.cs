namespace Platipus.Wallet.Api.Application.Services.OpenboxGameApi;

using Application.Requests.Wallets.Openbox.Base.Response;
using External;
using Results.HttpClient.WithData;

public interface IOpenboxGameApiClient
{
    Task<IResult<IHttpClientResult<string, OpenboxSingleResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        OpenboxGetLaunchScriptGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}