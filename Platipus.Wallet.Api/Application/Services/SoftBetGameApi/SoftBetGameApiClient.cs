namespace Platipus.Wallet.Api.Application.Services.SoftBetGameApi;

using System.Text.Json;
using Api.Extensions;
using Domain.Entities.Enums;
using External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Responses;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class SoftBetGameApiClient : ISoftBetGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "isoftbet/";

    public SoftBetGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(WalletProvider.SoftBet))
           .JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<string, SoftBetGsFailureResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        SoftBetGetLaunchUrlGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(apiRequest);

        return GetAsync<string>(
            baseUrl,
            "launch",
            queryParamsCollection,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, SoftBetGsFailureResponse>>> GetAsync<TSuccess>(
        Uri baseUrl,
        string methodRoute,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}/{methodRoute}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, methodRoute);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, SoftBetGsFailureResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, SoftBetGsFailureResponse>>(ErrorCode.UnknownHttpClientError);
        }
    }

    private IHttpClientResult<TSuccess, SoftBetGsFailureResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string methodRoute)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
            return httpResponse.Failure<TSuccess, SoftBetGsFailureResponse>();

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            if (methodRoute is "launch" && !responseBody.Contains("error"))
                return httpResponse.Success<TSuccess, SoftBetGsFailureResponse>((TSuccess)(object)responseBody);

            throw;
        }

        return methodRoute switch
        {
            "launch" => HandleLaunchGameResponse<TSuccess>(parsedJson, responseBody, httpResponse),
            _ => HandleDefaultResponse<TSuccess>(parsedJson, httpResponse)
        };
    }

    private IHttpClientResult<TSuccess, SoftBetGsFailureResponse> HandleLaunchGameResponse<TSuccess>(
        JsonDocument parsedJson,
        string responseBody,
        HttpClientRequest httpResponse)
    {
        if (!responseBody.Contains("error"))
            return httpResponse.Success<TSuccess, SoftBetGsFailureResponse>((TSuccess)(object)responseBody);

        var errorResponse = parsedJson.RootElement.Deserialize<SoftBetGsFailureResponse>(_jsonSerializerOptions);
        return errorResponse is not null
            ? httpResponse.Failure<TSuccess, SoftBetGsFailureResponse>(errorResponse)
            : httpResponse.Failure<TSuccess, SoftBetGsFailureResponse>();
    }

    private IHttpClientResult<TSuccess, SoftBetGsFailureResponse> HandleDefaultResponse<TSuccess>(
        JsonDocument parsedJson,
        HttpClientRequest httpResponse)
    {
        var root = parsedJson.RootElement;
        if (root.ValueKind == JsonValueKind.Object
         && root.TryGetProperty("error", out var error)
         && error.ValueKind is not JsonValueKind.Null)
        {
            var errorResponse = root.Deserialize<SoftBetGsFailureResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, SoftBetGsFailureResponse>(errorResponse);
        }

        var success = root.Deserialize<TSuccess>(_jsonSerializerOptions);
        return success is null
            ? httpResponse.Failure<TSuccess, SoftBetGsFailureResponse>()
            : httpResponse.Success<TSuccess, SoftBetGsFailureResponse>(success);
    }
}