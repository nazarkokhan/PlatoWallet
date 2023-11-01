namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi;

using System.Text;
using System.Text.Json;
using Abstaction;
using Api.Extensions.SecuritySign.Uranus;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Requests;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;
using JsonSerializer = System.Text.Json.JsonSerializer;

public sealed class UranusGameApiClient : IUranusGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "uranus/";
    private const string SecretKey = "67dqGQzHC4ue86Kb";

    public UranusGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(WalletProvider.Uranus))
           .JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<UranusSuccessResponse<UranusGameUrlData>, UranusFailureResponse>>>
        GetGameLaunchUrlAsync(
            Uri baseUrl,
            IUranusCommonGetLaunchUrlApiRequest apiRequest,
            CancellationToken cancellationToken = default)
    {
        return PostAsync<UranusSuccessResponse<UranusGameUrlData>, UranusGetLaunchUrlGameApiRequest>(
            baseUrl,
            "game/launch",
            (UranusGetLaunchUrlGameApiRequest)apiRequest,
            cancellationToken);
    }

    public Task<IResult<IHttpClientResult<UranusSuccessResponse<UranusAvailableGamesData>, UranusFailureResponse>>>
        GetAvailableGamesAsync(
            Uri baseUrl,
            UranusGetAvailableGamesGameApiRequest apiRequest,
            CancellationToken cancellationToken = default)
    {
        return PostAsync<UranusSuccessResponse<UranusAvailableGamesData>, UranusGetAvailableGamesGameApiRequest>(
            baseUrl,
            "game/list",
            apiRequest,
            cancellationToken);
    }

    public Task<IResult<IHttpClientResult<UranusSuccessResponse<UranusGameUrlData>, UranusFailureResponse>>>
        GetDemoLaunchUrlAsync(
            Uri baseUrl,
            IUranusCommonGetLaunchUrlApiRequest apiRequest,
            CancellationToken cancellationToken = default)
    {
        return PostAsync<UranusSuccessResponse<UranusGameUrlData>, UranusGetDemoLaunchUrlGameApiRequest>(
            baseUrl,
            "game/demo",
            (UranusGetDemoLaunchUrlGameApiRequest)apiRequest,
            cancellationToken);
    }

    public Task<IResult<IHttpClientResult<UranusSuccessResponse<string[]>, UranusFailureResponse>>> CreateCampaignAsync(
        Uri baseUrl,
        UranusCreateCampaignGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<UranusSuccessResponse<string[]>, UranusCreateCampaignGameApiRequest>(
            baseUrl,
            "freespins/create",
            apiRequest,
            cancellationToken);
    }

    public Task<IResult<IHttpClientResult<UranusSuccessResponse<string[]>, UranusFailureResponse>>> CancelCampaignAsync(
        Uri baseUrl,
        UranusCancelCampaignGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<UranusSuccessResponse<string[]>, UranusCancelCampaignGameApiRequest>(
            baseUrl,
            "freespin/cancel",
            apiRequest,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, UranusFailureResponse>>> PostAsync<TSuccess, TRequest>(
        Uri baseUrl,
        string method,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{method}");

            var requestContent = JsonSerializer.Serialize(request, _jsonSerializerOptions);
            var jsonContent = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var xSignature = UranusSecurityHash.Compute(requestContent, SecretKey);
            jsonContent.Headers.Add("X-Signature", xSignature);

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);
            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, UranusFailureResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, UranusFailureResponse>>(ErrorCode.UnknownHttpClientError, e);
        }
    }

    private IHttpClientResult<TSuccess, UranusFailureResponse> GetHttpResultAsync<TSuccess>(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;

            if (string.IsNullOrEmpty(responseBody))
            {
                return httpResponse.Failure<TSuccess, UranusFailureResponse>();
            }

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            if (responseJson.TryGetProperty("error", out var error) && !error.ValueKind.Equals(JsonValueKind.Null))
            {
                var errorResponse = responseJson.Deserialize<UranusFailureResponse>(_jsonSerializerOptions);
                if (errorResponse is not null)
                    return httpResponse.Failure<TSuccess, UranusFailureResponse>(errorResponse);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);

            return success is null
                ? httpResponse.Failure<TSuccess, UranusFailureResponse>()
                : httpResponse.Success<TSuccess, UranusFailureResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, UranusFailureResponse>(e);
        }
    }
}