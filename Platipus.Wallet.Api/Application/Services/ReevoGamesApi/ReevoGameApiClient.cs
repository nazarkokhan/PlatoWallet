namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using DTO;
using Domain.Entities.Enums;

public sealed class ReevoGameApiClient : IReevoGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _reevoJsonSerializerOptions;

    private const string ApiBasePath = "reevo/";

    public ReevoGameApiClient(HttpClient httpClient, IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _reevoJsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Reevo)).JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<ReevoGetGameGameApiResponse, ReevoErrorGameApiResponse>>> GetGameAsync(
        Uri baseUrl,
        ReevoGetGameGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoGetGameGameApiResponse>(
            baseUrl,
            request,
            "getgame",
            cancellationToken);
    }

    public async Task<IResult<IHttpClientResult<ReevoAddFreeRoundsGameApiResponse, ReevoErrorGameApiResponse>>>
        AddFreeRoundsAsync(
            Uri baseUrl,
            ReevoAddFreeRoundsGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoAddFreeRoundsGameApiResponse>(
            baseUrl,
            request,
            "addfreerounds",
            cancellationToken);
    }

    public async Task<IResult<IHttpClientResult<ReevoErrorGameApiResponse, ReevoErrorGameApiResponse>>> RemoveFreeRoundsAsync(
        Uri baseUrl,
        ReevoRemoveFreeRoundsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoErrorGameApiResponse>(
            baseUrl,
            request,
            "removefreerounds",
            cancellationToken);
    }

    public async Task<IResult<IHttpClientResult<ReevoGetGameHistoryGameApiResponse, ReevoErrorGameApiResponse>>>
        GetGameHistoryAsync(
            Uri baseUrl,
            ReevoGetGameHistoryGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoGetGameHistoryGameApiResponse>(
            baseUrl,
            request,
            "getgamehistory",
            cancellationToken);
    }

    public async Task<IResult<IHttpClientResult<ReevoGetGameListGameApiResponse, ReevoErrorGameApiResponse>>> GetGameListAsync(
        Uri baseUrl,
        ReevoGetGameListGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoGetGameListGameApiResponse>(
            baseUrl,
            request,
            "getgamelist",
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, ReevoErrorGameApiResponse>>> PostSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        object request,
        string methodRoute,
        CancellationToken cancellationToken)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{methodRoute.ToUpper()}");

            var jsonContent = JsonContent.Create(request, options: _reevoJsonSerializerOptions);

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);
            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, ReevoErrorGameApiResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, ReevoErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, ReevoErrorGameApiResponse> GetHttpResultAsync<TSuccess>(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;

            if (string.IsNullOrEmpty(responseBody))
            {
                return httpResponse.Failure<TSuccess, ReevoErrorGameApiResponse>();
            }

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            if (responseJson.TryGetProperty("error", out var error)
             && !error.ValueKind.Equals(JsonValueKind.Null)
             && error.GetInt32() is 1)
            {
                var errorResponse = responseJson.Deserialize<ReevoErrorGameApiResponse>(_reevoJsonSerializerOptions);
                if (errorResponse is not null)
                    return httpResponse.Failure<TSuccess, ReevoErrorGameApiResponse>(errorResponse);
            }

            var success = responseJson.Deserialize<TSuccess>(_reevoJsonSerializerOptions);

            return success is null
                ? httpResponse.Failure<TSuccess, ReevoErrorGameApiResponse>()
                : httpResponse.Success<TSuccess, ReevoErrorGameApiResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, ReevoErrorGameApiResponse>(e);
        }
    }
}