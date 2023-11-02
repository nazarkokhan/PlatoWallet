using System.Text.Json;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Application.Services.SweepiumGameApi.Requests;

namespace Platipus.Wallet.Api.Application.Services.SweepiumGameApi;

public sealed class SweepiumGameApiClient : ISweepiumGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    public Task<IResult<IHttpClientResult<string, SweepiumErrorResponse>>> LaunchAsync(Uri baseUrl, SweepiumGetLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}