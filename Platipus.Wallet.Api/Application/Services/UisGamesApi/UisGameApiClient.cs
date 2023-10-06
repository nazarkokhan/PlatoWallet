namespace Platipus.Wallet.Api.Application.Services.UisGamesApi;

using System.Text.Json;
using Api.Extensions;
using Dto;
using Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Domain.Entities.Enums;

public sealed class UisGameApiClient : IUisGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public UisGameApiClient(HttpClient httpClient, IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Uis)).JsonSerializerOptions;
    }

    //TODO wrong api. It is our wallet api, but should go directly to gameserver AwardBonus
    public async Task<IResult<IHttpClientResult<UisAwardBonusGameApiResponse, UisGameApiErrorResponse>>> AwardBonusAsync(
        Uri baseUrl,
        UisAwardBonusGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = new Dictionary<string, string?>
        {
            { "login", request.Login },
            { "password", request.Password },
            { "games", request.Games },
            { "fsquantity", request.Quantity },
            { "validUntil", request.ValidUntil },
            { "requestSign", request.RequestSign },
            { "env", request.Env }
        };

        return await GetAsync<UisAwardBonusGameApiResponse>(
            baseUrl,
            "award",
            queryParameters,
            cancellationToken);
    }

    public async Task<IResult<IHttpClientResult<UisCancelBonusGameApiResponse, UisGameApiErrorResponse>>> CancelBonusAsync(
        Uri baseUrl,
        UisCancelBonusGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = new Dictionary<string, string?>
        {
            { "login", request.Login },
            { "password", request.Password },
            { "bonus_id", request.BonusId },
            { "requestSign", request.RequestSign },
            { "env", request.Env }
        };

        return await GetAsync<UisCancelBonusGameApiResponse>(
            baseUrl,
            "bonus/cancel",
            queryParameters,
            cancellationToken);
    }

    public async Task<IResult<IHttpClientResult<string, UisGameApiErrorResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        UisGetLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = ObjectToDictionaryConverter.ConvertToDictionary(apiRequest);

        return await GetAsync<string>(
            baseUrl,
            "connect",
            queryParameters,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, UisGameApiErrorResponse>>> GetAsync<TSuccess>(
        Uri baseUrl,
        string method,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"uis/{method}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, method);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, UisGameApiErrorResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, UisGameApiErrorResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string method)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, UisGameApiErrorResponse>();

            if (method is "connect")
            {
                return (IHttpClientResult<TSuccess, UisGameApiErrorResponse>)HandleConnectResponse(responseBody, httpResponse);
            }

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            var isError = responseJson.TryGetProperty("statusCode", out var statusCode);
            if (isError)
            {
                var error = statusCode.Deserialize<UisGameApiErrorResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<TSuccess, UisGameApiErrorResponse>(error!);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            return success is null
                ? httpResponse.Failure<TSuccess, UisGameApiErrorResponse>()
                : httpResponse.Success<TSuccess, UisGameApiErrorResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, UisGameApiErrorResponse>(e);
        }
    }

    private static IHttpClientResult<string, UisGameApiErrorResponse> HandleConnectResponse(
        string responseBody,
        HttpClientRequest httpResponse)
    {
        return responseBody.Contains("Error", StringComparison.Ordinal)
            ? httpResponse.Failure<string, UisGameApiErrorResponse>()
            : httpResponse.Success<string, UisGameApiErrorResponse>(responseBody);
    }
}