using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi.Responses;
using Platipus.Wallet.Api.Extensions.SecuritySign;
using Platipus.Wallet.Domain.Entities.Enums;

namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;

public sealed class EmaraPlayGameApiClient : IEmaraPlayGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly JsonSerializerSettings _jsonSettings;
    
    private const string ApiBasePath = "emaraplay/";

    public EmaraPlayGameApiClient(
        HttpClient httpClient, 
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(CasinoProvider.EmaraPlay))
            .JsonSerializerOptions;
        _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
    }

    public Task<IResult<IHttpClientResult<EmaraPlayGetLauncherUrlResponse, EmaraPlayGameApiErrorResponse>>> GetLauncherUrlAsync(
        Uri baseUrl, EmaraPlayGetLauncherUrlRequest urlRequest, CancellationToken cancellationToken = default)
    {
        return PostAsync<EmaraPlayGetLauncherUrlResponse>(
            baseUrl, "launcher", urlRequest, cancellationToken);
    }


    private async Task<IResult<IHttpClientResult<TSuccess, EmaraPlayGameApiErrorResponse>>> PostAsync<TSuccess>(
        Uri baseUrl,
        string method,
        EmaraPlayGetLauncherUrlRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{method}");
            
            var requestToSend = request.Mode != "real_play" 
                ? request with { Token = null!, User = null }
                : request;

            var requestContent = JsonConvert.SerializeObject(requestToSend, _jsonSettings);
            var content = new StringContent(requestContent, Encoding.UTF8, "application/json");
            
            var hash = EmaraPlaySecurityHash.Compute(Encoding.UTF8.GetBytes(requestContent), "12345678");

            // Add Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hash.ToLower());

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, content, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, EmaraPlayGameApiErrorResponse>>(
                ErrorCode.UnknownHttpClientError, e);
        }
    }
    
    private async Task<IResult<IHttpClientResult<TSuccess, EmaraPlayGameApiErrorResponse>>> GetAsync<TSuccess>(
        Uri baseUrl,
        string method,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"emaraplay/{method}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, EmaraPlayGameApiErrorResponse>>(
                ErrorCode.UnknownHttpClientError, e);
        }
    }

    private IHttpClientResult<TSuccess, EmaraPlayGameApiErrorResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, EmaraPlayGameApiErrorResponse>();

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            return success is null ? 
                httpResponse.Failure<TSuccess, EmaraPlayGameApiErrorResponse>() : 
                httpResponse.Success<TSuccess, EmaraPlayGameApiErrorResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, EmaraPlayGameApiErrorResponse>(e);
        }
    }
    
}