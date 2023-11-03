namespace Platipus.Wallet.Api.Application.Services.SweepiumGameApi;

using System.Text.Json;
using Api.Extensions;
using Domain.Entities.Enums;
using External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Responses.Sweepium.Base;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class SweepiumGameApiClient : ISweepiumGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "sweepium/";

    public SweepiumGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(WalletProvider.Sweepium))
           .JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<string, SweepiumErrorResponse>>> LaunchAsync(
        Uri baseUrl,
        SweepiumGetLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(apiRequest);

        var collectionWithoutNulls =
            queryParamsCollection.Where(kvp => kvp.Value is not null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return GetRequestAsync<string>(
            baseUrl,
            "game/launch",
            collectionWithoutNulls,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, SweepiumErrorResponse>>> GetRequestAsync<TSuccess>(
        Uri baseUrl,
        string method,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{method}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, SweepiumErrorResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, SweepiumErrorResponse>>(ErrorCode.UnknownHttpClientError);
        }
    }

    private IHttpClientResult<TSuccess, SweepiumErrorResponse> GetHttpResultAsync<TSuccess>(HttpClientRequest httpResponse)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
        {
            return httpResponse.Failure<TSuccess, SweepiumErrorResponse>();
        }

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            return httpResponse.Success<TSuccess, SweepiumErrorResponse>((TSuccess)(object)responseBody);
        }

        if (parsedJson.RootElement.TryGetProperty("result", out var result)
         && !result.ValueKind.Equals(JsonValueKind.Null)
         && result.GetBoolean() is false)
        {
            var errorResponse = parsedJson.Deserialize<SweepiumErrorResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, SweepiumErrorResponse>(errorResponse);
        }

        return httpResponse.Failure<TSuccess, SweepiumErrorResponse>();
    }
}