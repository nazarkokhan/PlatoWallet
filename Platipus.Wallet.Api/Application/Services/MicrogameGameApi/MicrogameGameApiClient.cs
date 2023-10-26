namespace Platipus.Wallet.Api.Application.Services.MicrogameGameApi;

using System.Text.Json;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Requests;
using Responses.Microgame.Base;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class MicrogameGameApiClient : IMicrogameGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "microgame/";

    public MicrogameGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(WalletProvider.Microgame))
           .JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<string, MicrogameErrorResponse>>> LaunchAsync(
        Uri baseUrl,
        MicrogameLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        const string methodRoute = "game/launch";
        return await PostRequestAsync<string>(
            baseUrl,
            methodRoute,
            apiRequest,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, MicrogameErrorResponse>>> PostRequestAsync<TSuccess>(
        Uri baseUrl,
        string methodRoute,
        object request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}/{methodRoute}");

            var jsonContent = JsonContent.Create(request, options: _jsonSerializerOptions);

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, methodRoute);
            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, MicrogameErrorResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, MicrogameErrorResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, MicrogameErrorResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string methodRoute)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
            return httpResponse.Failure<TSuccess, MicrogameErrorResponse>();

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            if (string.Equals(methodRoute, "game/launch"))
                return httpResponse.Success<TSuccess, MicrogameErrorResponse>((TSuccess)(object)responseBody);

            throw;
        }

        return HandleDefaultResponse<TSuccess>(parsedJson, httpResponse);
    }

    private IHttpClientResult<TSuccess, MicrogameErrorResponse> HandleDefaultResponse<TSuccess>(
        JsonDocument parsedJson,
        HttpClientRequest httpResponse)
    {
        var root = parsedJson.RootElement;
        if (root.ValueKind == JsonValueKind.Object
         && root.TryGetProperty("error", out var error)
         && error.ValueKind is not JsonValueKind.Null)
        {
            var errorResponse = root.Deserialize<MicrogameErrorResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, MicrogameErrorResponse>(errorResponse);
        }

        var success = root.Deserialize<TSuccess>(_jsonSerializerOptions);
        return success is null
            ? httpResponse.Failure<TSuccess, MicrogameErrorResponse>()
            : httpResponse.Success<TSuccess, MicrogameErrorResponse>(success);
    }
}