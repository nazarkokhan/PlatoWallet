namespace Platipus.Wallet.Api.Application.Services.ParimatchGameApi;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Extensions;
using Domain.Entities.Enums;
using Requests;
using Responses;

public class ParimatchGameApiClient : IParimatchGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ParimatchGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Parimatch)).JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<string, ParimatchErrorGameApiResponse>>>
        LauncherAsync(
            Uri baseUrl,
            ParimatchLauncherGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(request);
            var queryString = QueryString.Create(queryParamsCollection);

            baseUrl = new Uri(baseUrl, "parimatch/launcher" + queryString);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl);

            var httpResponseOriginal = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<string, ParimatchErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    public async Task<IResult<IHttpClientResult<ParimatchCreateAwardGameApiResponse, ParimatchErrorGameApiResponse>>>
        CreateAwardAsync(
            Uri baseUrl,
            ParimatchCreateAwardGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response =
            await PostSignedRequestAsync<ParimatchCreateAwardGameApiResponse>(
                baseUrl,
                "award",
                request,
                cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<ParimatchDeleteAwardGameApiResponse, ParimatchErrorGameApiResponse>>>
        DeleteAwardAsync(
            Uri baseUrl,
            ParimatchDeleteAwardGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<ParimatchDeleteAwardGameApiResponse>(
            baseUrl,
            "cancel",
            request,
            cancellationToken);

        return response;
    }

    private async Task<IResult<IHttpClientResult<TSuccess, ParimatchErrorGameApiResponse>>> PostSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        string method,
        object request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonContent = JsonContent.Create(request, options: _jsonSerializerOptions);

            baseUrl = new Uri(baseUrl, $"parimatch/{method}");

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, ParimatchErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, ParimatchErrorGameApiResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, ParimatchErrorGameApiResponse>();

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            var isError = responseJson.TryGetProperty("error", out var errorCode)
                       && errorCode.GetInt32() != 0;
            if (isError)
            {
                var error = responseJson.Deserialize<ParimatchErrorGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<TSuccess, ParimatchErrorGameApiResponse>(error!);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            if (success is null)
                return httpResponse.Failure<TSuccess, ParimatchErrorGameApiResponse>();

            return httpResponse.Success<TSuccess, ParimatchErrorGameApiResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, ParimatchErrorGameApiResponse>(e);
        }
    }

    private IHttpClientResult<string, ParimatchErrorGameApiResponse> GetHttpResultAsync(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<string, ParimatchErrorGameApiResponse>();

            JsonElement? responseJson;
            try
            {
                responseJson = JsonDocument.Parse(responseBody).RootElement;
            }
            catch
            {
                responseJson = null;
            }

            var isError = responseJson?.ValueKind is JsonValueKind.Object
                       && responseJson.Value.TryGetProperty("error", out _);
            if (isError)
            {
                var error = responseJson!.Value.Deserialize<ParimatchErrorGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<string, ParimatchErrorGameApiResponse>(error!);
            }

            return httpResponse.Success<string, ParimatchErrorGameApiResponse>(responseBody);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<string, ParimatchErrorGameApiResponse>(e);
        }
    }
}