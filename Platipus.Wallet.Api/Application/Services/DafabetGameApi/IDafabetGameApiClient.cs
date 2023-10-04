namespace Platipus.Wallet.Api.Application.Services.DafabetGameApi;

using Application.Requests.Wallets.Dafabet.Base.Response;
using Requests;
using Results.HttpClient.WithData;

public interface IDafabetGameApiClient
{
    Task<IResult<IHttpClientResult<string, DafabetErrorResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        string signatureKey,
        DafabetGetLaunchUrlGameApiRequest apiRequest,
        CancellationToken cancellationToken = default);
}