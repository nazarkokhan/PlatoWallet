namespace Platipus.Wallet.Api.Application.Services.AtlasGamesApi;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Requests.Wallets.Atlas.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Requests;
using Responses.AtlasPlatform;
using Results.Atlas;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class AtlasGameApiClient : IAtlasGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "atlas/";

    public AtlasGameApiClient(
        HttpClient httpClient, 
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(CasinoProvider.Atlas))
            .JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<AtlasLaunchGameResponse, AtlasErrorResponse>>> LaunchGameAsync(
        Uri baseUrl, 
        AtlasGameLaunchGameApiRequest apiRequest,
        string token,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<AtlasLaunchGameResponse, AtlasGameLaunchGameApiRequest>(
            baseUrl, "gamelaunch", apiRequest, token, cancellationToken);
    }

    public Task<IResult<IHttpClientResult<AtlasGetGamesListResponse, AtlasErrorResponse>>> GetGamesListAsync(
        Uri baseUrl, 
        AtlasGetGamesListGameApiRequest apiRequest,
        string token,
        CancellationToken cancellationToken = default)
    {
        var request = new Dictionary<string, string?>
        {
            {nameof(apiRequest.CasinoId), apiRequest.CasinoId}
        };
        return GetAsync<AtlasGetGamesListResponse>(
            baseUrl, "getgames", request, token, cancellationToken);
    }

    public Task<IResult<IHttpClientResult<IAtlasResult, AtlasErrorResponse>>> RegisterFreeSpinBonusAsync(
        Uri baseUrl, 
        AtlasRegisterFreeSpinBonusGameApiRequest apiRequest, 
        string token,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<IAtlasResult, AtlasRegisterFreeSpinBonusGameApiRequest>(
            baseUrl, "registerBonus", apiRequest, token, cancellationToken);
    }

    public Task<IResult<IHttpClientResult<IAtlasResult, AtlasErrorResponse>>> AssignFreeSpinBonusAsync(
        Uri baseUrl, 
        AtlasAssignFreeSpinBonusGameApiRequest apiRequest, 
        string token,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<IAtlasResult, AtlasAssignFreeSpinBonusGameApiRequest>(
            baseUrl, "assignBonus", apiRequest, token, cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, AtlasErrorResponse>>> PostAsync<TSuccess, TRequest>(
        Uri baseUrl,
        string method,
        TRequest request,
        string? token,
        CancellationToken cancellationToken = default)
    where TRequest : class
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{method}");

            var requestContent = JsonConvert.SerializeObject(request);
            var content = new StringContent(requestContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", token);

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, content, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);
            if (httpResult.IsFailure)
            {
                return ResultFactory.Failure<IHttpClientResult<TSuccess, AtlasErrorResponse>>(
                    ErrorCode.Unknown);
            }

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, AtlasErrorResponse>>(
                ErrorCode.UnknownHttpClientError, e);
        }
    }

    private async Task<IResult<IHttpClientResult<TSuccess, AtlasErrorResponse>>> GetAsync<TSuccess>(
        Uri baseUrl,
        string method,
        Dictionary<string, string?> request,
        string? token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}/{method}{QueryString.Create(request)}");
           
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, AtlasErrorResponse>>(
                ErrorCode.UnknownHttpClientError, e);
        }
    }

    private IHttpClientResult<TSuccess, AtlasErrorResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (string.IsNullOrEmpty(responseBody))
                return httpResponse.Failure<TSuccess, AtlasErrorResponse>();

            var responseJson = JsonDocument.Parse(responseBody).RootElement;
            
            if (responseJson.TryGetProperty("error", out _) 
                && responseJson.TryGetProperty("errorCode", out _))
            {
                var errorResponse = responseJson.Deserialize<AtlasErrorResponse>(_jsonSerializerOptions);
                if (errorResponse is not null) 
                    return httpResponse.Failure<TSuccess, AtlasErrorResponse>(errorResponse);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            
            return success is null ? 
                httpResponse.Failure<TSuccess, AtlasErrorResponse>() : 
                httpResponse.Success<TSuccess, AtlasErrorResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, AtlasErrorResponse>(e);
        }
    }
}