namespace Platipus.Wallet.Api.Application.Services.EvenbetGameApi;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet.Models;
using Responses.Evenbet.Base;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Extensions;
using Domain.Entities.Enums;
using Requests;

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
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(WalletProvider.Evenbet))
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

    public Task<IResult<IHttpClientResult<string, EvenbetFailureResponse>>> GetGameLaunchUrlAsync(
        Uri baseUrl,
        EvenbetGetLaunchGameUrlGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var requestToServer = new Dictionary<string, string?>
        {
            { nameof(request.Token).ToCamelCase(), request.Token },
            { nameof(request.Currency).ToCamelCase(), request.Currency },
            { nameof(request.CasinoId).ToCamelCase(), request.CasinoId },
            { nameof(request.Language).ToCamelCase(), request.Language },
            { nameof(request.GameId).ToCamelCase(), request.GameId },
            { nameof(request.Platform).ToCamelCase(), request.Platform },
            { nameof(request.Mode).ToCamelCase(), request.Mode ? "1" : "0" },
        };

        return GetAsync<string>(
            baseUrl,
            "game/launch",
            requestToServer,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, EvenbetFailureResponse>>> GetAsync<TSuccess>(
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
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, EvenbetFailureResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, EvenbetFailureResponse>>(ErrorCode.UnknownHttpClientError);
        }
    }

    private IHttpClientResult<TSuccess, EvenbetFailureResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string methodRoute)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
            return httpResponse.Failure<TSuccess, EvenbetFailureResponse>();

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            if (methodRoute is "game/launch" && !responseBody.Contains("error"))
                return httpResponse.Success<TSuccess, EvenbetFailureResponse>((TSuccess)(object)responseBody);

            throw;
        }

        return methodRoute switch
        {
            "game/launch" => HandleLaunchGameResponse<TSuccess>(parsedJson, responseBody, httpResponse),
            _ => HandleDefaultResponse<TSuccess>(parsedJson, httpResponse)
        };
    }

    private IHttpClientResult<TSuccess, EvenbetFailureResponse> HandleLaunchGameResponse<TSuccess>(
        JsonDocument parsedJson,
        string responseBody,
        HttpClientRequest httpResponse)
    {
        if (!responseBody.Contains("error"))
            return httpResponse.Success<TSuccess, EvenbetFailureResponse>((TSuccess)(object)responseBody);

        var errorResponse = parsedJson.RootElement.Deserialize<EvenbetFailureResponse>(_jsonSerializerOptions);
        return errorResponse is not null
            ? httpResponse.Failure<TSuccess, EvenbetFailureResponse>(errorResponse)
            : httpResponse.Failure<TSuccess, EvenbetFailureResponse>();
    }

    private IHttpClientResult<TSuccess, EvenbetFailureResponse> HandleDefaultResponse<TSuccess>(
        JsonDocument parsedJson,
        HttpClientRequest httpResponse)
    {
        var root = parsedJson.RootElement;
        if (root.ValueKind == JsonValueKind.Object
         && root.TryGetProperty("error", out var error)
         && error.ValueKind is not JsonValueKind.Null)
        {
            var errorResponse = root.Deserialize<EvenbetFailureResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, EvenbetFailureResponse>(errorResponse);
        }

        var success = root.Deserialize<TSuccess>(_jsonSerializerOptions);
        return success is null
            ? httpResponse.Failure<TSuccess, EvenbetFailureResponse>()
            : httpResponse.Success<TSuccess, EvenbetFailureResponse>(success);
    }
}