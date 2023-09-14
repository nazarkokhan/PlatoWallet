namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Domain.Entities.Enums;
using Requests;
using Responses;

public class EverymatrixGameApiClient : IEverymatrixGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public EverymatrixGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Everymatrix)).JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<EverymatrixAwardGameApiResponse, EverymatrixErrorGameApiResponse>>>
        CreateAwardAsync(
            Uri baseUrl,
            EverymatrixCreateAwardGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<EverymatrixAwardGameApiResponse>(
            baseUrl,
            "awardbonus",
            request,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<EverymatrixAwardGameApiResponse, EverymatrixErrorGameApiResponse>>>
        DeleteAwardAsync(
            Uri baseUrl,
            EverymatrixDeleteAwardGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<EverymatrixAwardGameApiResponse>(
            baseUrl,
            "foreitbonus",
            request,
            cancellationToken);

        return response;
    }

    private async Task<IResult<IHttpClientResult<TSuccess, EverymatrixErrorGameApiResponse>>>
        PostSignedRequestAsync<TSuccess>(
            Uri requestUrl,
            string method,
            object request,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonContent = JsonContent.Create(request, options: _jsonSerializerOptions);

            requestUrl = new Uri(requestUrl, $"everymatrix/{method}");

            var httpResponseOriginal = await _httpClient.PostAsync(requestUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, EverymatrixErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, EverymatrixErrorGameApiResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, EverymatrixErrorGameApiResponse>();

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            var isError = responseJson.TryGetProperty("Success", out var successStatus);
            if (isError && !successStatus.GetBoolean())
            {
                var error = responseJson.Deserialize<EverymatrixErrorGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<TSuccess, EverymatrixErrorGameApiResponse>(error!);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            if (success is null)
                return httpResponse.Failure<TSuccess, EverymatrixErrorGameApiResponse>();

            return httpResponse.Success<TSuccess, EverymatrixErrorGameApiResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, EverymatrixErrorGameApiResponse>(e);
        }
    }
}