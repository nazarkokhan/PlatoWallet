namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi;

using System.Text.Json;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain.Entities.Enums;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class ReevoGameApiClient : IReevoGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _reevoJsonSerializerOptions;

    private const string ApiBasePath = "reevo";

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
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, ReevoErrorGameApiResponse>>> PostSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        object request,
        CancellationToken cancellationToken)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}");

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

            if (responseJson.TryGetProperty("error", out var error) && !error.ValueKind.Equals(JsonValueKind.Null))
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