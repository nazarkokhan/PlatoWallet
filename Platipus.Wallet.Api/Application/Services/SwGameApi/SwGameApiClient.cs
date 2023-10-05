namespace Platipus.Wallet.Api.Application.Services.SwGameApi;

using System.Text.Json;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Extensions;
using Requests;
using Responses;

public sealed class SwGameApiClient : ISwGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SwGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Sw)).JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<SwAwardGameApiResponse, object>>> CreateFreespin(
        Uri baseUrl,
        SwCreateAwardGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response =
            await PostSignedRequestAsync<SwCreateAwardGameApiRequest, SwAwardGameApiResponse>(
                baseUrl,
                "FREESPIN.DO",
                request,
                cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<SwAwardGameApiResponse, object>>> DeleteFreespin(
        Uri baseUrl,
        SwDeleteFreespinGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<SwDeleteFreespinGameApiRequest, SwAwardGameApiResponse>(
            baseUrl,
            "DELETEFREESPIN.DO",
            request,
            cancellationToken);

        return response;
    }

    public Task<IResult<IHttpClientResult<string, object>>> GetLaunchScriptAsync(
        Uri baseUrl,
        SwGetLaunchUrlGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(apiRequest);

        return GetSignedRequestAsync<string>(
            baseUrl,
            "CONNECT.DO",
            queryParamsCollection,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, object>>> GetSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        string method,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"BIGBOSS/{method}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, method);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, object>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, object>>(ErrorCode.UnknownHttpClientError);
        }
    }

    private async Task<IResult<IHttpClientResult<TSuccess, object>>>
        PostSignedRequestAsync<TRequest, TSuccess>(
            Uri baseUrl,
            string method,
            TRequest queryRequest,
            CancellationToken cancellationToken = default)
        where TRequest : class
    {
        try
        {
            var queryDictionary = ObjectToDictionaryConverter.ConvertToDictionary(queryRequest);

            var queryString = QueryString.Create(queryDictionary);
            baseUrl = new Uri(baseUrl, $"BIGBOSS/{method}{queryString}");

            var httpResponseOriginal = await _httpClient.PostAsJsonAsync(baseUrl, (object?)null, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, method);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, object>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, object> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string method)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, object>();

            // Handle CONNECT.DO method differently.
            if (method is "CONNECT.DO")
            {
                return (IHttpClientResult<TSuccess, object>)HandleConnectDoResponse(responseBody, httpResponse);
            }
            
            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            var isError = responseJson.TryGetProperty("result", out var resultStatus);
            if (isError && resultStatus.ToString() == "ERROR")
            {
                var error = responseJson.Deserialize<SwErrorGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<TSuccess, object>(error!);
            }

            isError = responseJson.TryGetProperty("statusCode", out var statusCodeStatus);
            if (isError && statusCodeStatus.GetInt32() is not 0)
            {
                var error = responseJson.Deserialize<SwErrorEngineGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<TSuccess, object>(error!);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            return success is null
                ? httpResponse.Failure<TSuccess, object>()
                : httpResponse.Success<TSuccess, object>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, object>(e);
        }
    }

    private static IHttpClientResult<string, object> HandleConnectDoResponse(
        string responseBody,
        HttpClientRequest httpResponse)
    {
        return responseBody.Contains("Error", StringComparison.OrdinalIgnoreCase)
            ? httpResponse.Failure<string, object>()
            : httpResponse.Success<string, object>(responseBody);
    }
}