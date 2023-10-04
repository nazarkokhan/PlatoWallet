namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi;

using System.Text.Json;
using System.Text.RegularExpressions;
using Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Domain.Entities.Enums;
using External;
using Requests;
using Responses;

public sealed class EverymatrixGameApiClient : IEverymatrixGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "everymatrix/";

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
            "forfeitbonus",
            request,
            cancellationToken);

        return response;
    }

    public Task<IResult<IHttpClientResult<string, EverymatrixErrorGameApiResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        EverymatrixGetLaunchUrlGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(request);

        return GetSignedRequestAsync<string>(
            baseUrl,
            "launch",
            queryParamsCollection,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, EverymatrixErrorGameApiResponse>>> GetSignedRequestAsync<TSuccess>(
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

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, method);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, EverymatrixErrorGameApiResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, EverymatrixErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError);
        }
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

            requestUrl = new Uri(requestUrl, $"{ApiBasePath}{method}");

            var httpResponseOriginal = await _httpClient.PostAsync(requestUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, method);

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
        HttpClientRequest httpResponse,
        string method)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
            return httpResponse.Failure<TSuccess, EverymatrixErrorGameApiResponse>();

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            // Adjust the condition according to the Everymatrix API response format.
            var regex = new Regex("\"Success\"\\s*:\\s*false");
            if (method is "launch" && !regex.IsMatch(responseBody))
                return httpResponse.Success<TSuccess, EverymatrixErrorGameApiResponse>((TSuccess)(object)responseBody);

            throw;
        }

        return method switch
        {
            "launch" => HandleLaunchGameResponse<TSuccess>(parsedJson, responseBody, httpResponse),
            _ => HandleDefaultResponse<TSuccess>(parsedJson, httpResponse)
        };
    }

    private IHttpClientResult<TSuccess, EverymatrixErrorGameApiResponse> HandleLaunchGameResponse<TSuccess>(
        JsonDocument parsedJson,
        string responseBody,
        HttpClientRequest httpResponse)
    {
        // Get the "Success" property from the JSON.
        var isSuccess = parsedJson.RootElement.TryGetProperty("Success", out var success) && success.GetBoolean();
        if (isSuccess)
            return httpResponse.Success<TSuccess, EverymatrixErrorGameApiResponse>((TSuccess)(object)responseBody);

        var errorResponse = parsedJson.RootElement.Deserialize<EverymatrixErrorGameApiResponse>(_jsonSerializerOptions);
        return errorResponse is not null
            ? httpResponse.Failure<TSuccess, EverymatrixErrorGameApiResponse>(errorResponse)
            : httpResponse.Failure<TSuccess, EverymatrixErrorGameApiResponse>();
    }

    private IHttpClientResult<TSuccess, EverymatrixErrorGameApiResponse> HandleDefaultResponse<TSuccess>(
        JsonDocument parsedJson,
        HttpClientRequest httpResponse)
    {
        var root = parsedJson.RootElement;

        // Get the "Success" property from the JSON.
        if (root.ValueKind == JsonValueKind.Object
         && root.TryGetProperty("Success", out var success)
         && success.ValueKind is not JsonValueKind.Null
         && !success.GetBoolean())
        {
            var errorResponse = root.Deserialize<EverymatrixErrorGameApiResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, EverymatrixErrorGameApiResponse>(errorResponse);
        }

        var responseSuccess = root.Deserialize<TSuccess>(_jsonSerializerOptions);
        return responseSuccess is null
            ? httpResponse.Failure<TSuccess, EverymatrixErrorGameApiResponse>()
            : httpResponse.Success<TSuccess, EverymatrixErrorGameApiResponse>(responseSuccess);
    }
}