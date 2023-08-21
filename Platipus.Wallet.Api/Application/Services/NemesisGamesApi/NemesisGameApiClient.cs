namespace Platipus.Wallet.Api.Application.Services.NemesisGamesApi;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;
using Platipus.Wallet.Api.Extensions;
using Domain.Entities.Enums;
using Responses;

public class NemesisGameApiClient : INemesisGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public NemesisGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Nemesis)).JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<NemesisLauncherGameApiResponse, NemesisErrorGameApiResponse>>>
        LauncherAsync(
            Uri baseUrl,
            NemesisLauncherGameApiRequest request,
            string xIntegrationToken,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParameters = ObjectToDictionaryConverter.ConvertToDictionary(request);
            var queryString = QueryString.Create(queryParameters);
            baseUrl = new Uri(baseUrl, "launcher" + queryString);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl)
            {
                Headers =
                {
                    { NemesisHeaders.XIntegrationToken, xIntegrationToken }
                }
            };

            var httpResponseOriginal = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<NemesisLauncherGameApiResponse>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<NemesisLauncherGameApiResponse, NemesisErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    public async Task<IResult<IHttpClientResult<NemesisCreateAwardGameApiResponse, NemesisErrorGameApiResponse>>>
        CreateAwardAsync(
            Uri baseUrl,
            NemesisCreateAwardGameApiRequest request,
            string xIntegrationToken,
            CancellationToken cancellationToken = default)
    {
        var response =
            await PostSignedRequestAsync<NemesisCreateAwardGameApiResponse>(
                baseUrl,
                "award",
                request,
                xIntegrationToken,
                cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<NemesisCancelAwardGameApiResponse, NemesisErrorGameApiResponse>>>
        CancelAwardAsync(
            Uri baseUrl,
            NemesisCancelAwardGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<NemesisCancelAwardGameApiResponse>(
            baseUrl,
            "cancel",
            request,
            null,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<NemesisCurrencyGameApiResponse, NemesisErrorGameApiResponse>>> Currency(
        Uri baseUrl,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<NemesisCurrencyGameApiResponse>(
            baseUrl,
            "currency",
            null,
            null,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<NemesisRoundGameApiResponse, NemesisErrorGameApiResponse>>> RoundGame(
        Uri baseUrl,
        NemesisRoundGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<NemesisRoundGameApiResponse>(
            baseUrl,
            "currency",
            request,
            null,
            cancellationToken);

        return response;
    }

    private async Task<IResult<IHttpClientResult<TSuccess, NemesisErrorGameApiResponse>>> PostSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        string method,
        object? request,
        string? xIntegrationToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonContent = JsonContent.Create(request, options: _jsonSerializerOptions);

            if (xIntegrationToken is not null)
                jsonContent.Headers.Add(NemesisHeaders.XIntegrationToken, xIntegrationToken);

            baseUrl = new Uri(baseUrl, method);

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, NemesisErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, NemesisErrorGameApiResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, NemesisErrorGameApiResponse>();

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            var isError = responseJson.TryGetProperty("error", out var errorCode);
            if (isError)
            {
                var error = responseJson.Deserialize<NemesisErrorGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<TSuccess, NemesisErrorGameApiResponse>(error!);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            if (success is null)
                return httpResponse.Failure<TSuccess, NemesisErrorGameApiResponse>();

            return httpResponse.Success<TSuccess, NemesisErrorGameApiResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, NemesisErrorGameApiResponse>(e);
        }
    }
}