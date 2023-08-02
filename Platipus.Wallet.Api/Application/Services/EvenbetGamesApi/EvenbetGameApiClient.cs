namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi;

using System.Text;
using System.Text.Json;
using Application.Requests.Wallets.Evenbet.Models;
using Domain.Entities.Enums;
using External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Requests;
using Responses.Evenbet.Base;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

internal sealed class EvenbetGameApiClient : IEvenbetGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "evenbet/";

    public EvenbetGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(CasinoProvider.Evenbet))
           .JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<List<EvenbetGameModel>, EvenbetFailureResponse>>> GetGamesAsync(
        Uri baseUrl,
        CancellationToken cancellationToken = default)
    {
        var request = new Dictionary<string, string?>();

        return GetAsync<List<EvenbetGameModel>>(
            baseUrl,
            "game/list",
            request,
            cancellationToken);
    }

    public Task<IResult<IHttpClientResult<EvenbetGetLaunchGameUrlResponse, EvenbetFailureResponse>>> GetGameLaunchUrlAsync(
        Uri baseUrl,
        EvenbetGetLaunchGameUrlGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<EvenbetGetLaunchGameUrlResponse, EvenbetGetLaunchGameUrlGameApiRequest>(
            baseUrl,
            "game/launch",
            request,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, EvenbetFailureResponse>>> PostAsync<TSuccess, TRequest>(
        Uri baseUrl,
        string method,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{method}");

            var requestContent = JsonConvert.SerializeObject(request);
            var jsonContent = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);
            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, EvenbetFailureResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, EvenbetFailureResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private async Task<IResult<IHttpClientResult<TSuccess, EvenbetFailureResponse>>> GetAsync<TSuccess>(
        Uri baseUrl,
        string method,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}/{method}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, EvenbetFailureResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, EvenbetFailureResponse>>(ErrorCode.UnknownHttpClientError);
        }
    }

    private IHttpClientResult<TSuccess, EvenbetFailureResponse> GetHttpResultAsync<TSuccess>(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;

            if (string.IsNullOrEmpty(responseBody))
            {
                return httpResponse.Failure<TSuccess, EvenbetFailureResponse>();
            }

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            if (responseJson.ValueKind is JsonValueKind.Object
             && responseJson.TryGetProperty("error", out var error)
             && !error.ValueKind.Equals(JsonValueKind.Null))
            {
                var errorResponse = responseJson.Deserialize<EvenbetFailureResponse>(_jsonSerializerOptions);
                if (errorResponse is not null)
                    return httpResponse.Failure<TSuccess, EvenbetFailureResponse>(errorResponse);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);

            return success is null
                ? httpResponse.Failure<TSuccess, EvenbetFailureResponse>()
                : httpResponse.Success<TSuccess, EvenbetFailureResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, EvenbetFailureResponse>(e);
        }
    }
}