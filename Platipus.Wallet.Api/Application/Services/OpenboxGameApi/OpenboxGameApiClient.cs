namespace Platipus.Wallet.Api.Application.Services.OpenboxGameApi;

using System.Text.Json;
using Api.Extensions;
using Application.Requests.Wallets.Openbox.Base.Response;
using Domain.Entities.Enums;
using External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class OpenboxGameApiClient : IOpenboxGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "openbox/";

    public OpenboxGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Openbox)).JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<string, OpenboxSingleResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        OpenboxGetLaunchScriptGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(apiRequest);

        return GetSignedRequestAsync<string>(
            baseUrl,
            "launcher",
            queryParamsCollection,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, OpenboxSingleResponse>>> GetSignedRequestAsync<TSuccess>(
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
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, OpenboxSingleResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, OpenboxSingleResponse>>(ErrorCode.UnknownHttpClientError);
        }
    }

    private IHttpClientResult<TSuccess, OpenboxSingleResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string method)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
            return httpResponse.Failure<TSuccess, OpenboxSingleResponse>();

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            if (method is "launcher" && !responseBody.Contains("error"))
                return httpResponse.Success<TSuccess, OpenboxSingleResponse>((TSuccess)(object)responseBody);

            throw;
        }

        return method switch
        {
            "launcher" => HandleLaunchGameResponse<TSuccess>(parsedJson, responseBody, httpResponse),
            _ => HandleDefaultResponse<TSuccess>(parsedJson, httpResponse)
        };
    }

    private IHttpClientResult<TSuccess, OpenboxSingleResponse> HandleLaunchGameResponse<TSuccess>(
        JsonDocument parsedJson,
        string responseBody,
        HttpClientRequest httpResponse)
    {
        if (!responseBody.Contains("error"))
            return httpResponse.Success<TSuccess, OpenboxSingleResponse>((TSuccess)(object)responseBody);

        var errorResponse = parsedJson.RootElement.Deserialize<OpenboxSingleResponse>(_jsonSerializerOptions);
        return errorResponse is not null
            ? httpResponse.Failure<TSuccess, OpenboxSingleResponse>(errorResponse)
            : httpResponse.Failure<TSuccess, OpenboxSingleResponse>();
    }

    private IHttpClientResult<TSuccess, OpenboxSingleResponse> HandleDefaultResponse<TSuccess>(
        JsonDocument parsedJson,
        HttpClientRequest httpResponse)
    {
        var root = parsedJson.RootElement;
        if (root.ValueKind == JsonValueKind.Object
         && root.TryGetProperty("error", out var error)
         && error.ValueKind is not JsonValueKind.Null)
        {
            var errorResponse = root.Deserialize<OpenboxSingleResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, OpenboxSingleResponse>(errorResponse);
        }

        var success = root.Deserialize<TSuccess>(_jsonSerializerOptions);
        return success is null
            ? httpResponse.Failure<TSuccess, OpenboxSingleResponse>()
            : httpResponse.Success<TSuccess, OpenboxSingleResponse>(success);
    }
}