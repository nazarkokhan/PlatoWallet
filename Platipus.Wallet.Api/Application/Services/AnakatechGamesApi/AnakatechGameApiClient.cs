namespace Platipus.Wallet.Api.Application.Services.AnakatechGamesApi;

using System.Text;
using System.Text.Json;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Requests;
using Responses.Anakatech.Base;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class AnakatechGameApiClient : IAnakatechGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "anakatech/";

    public AnakatechGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(CasinoProvider.Anakatech))
           .JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<Stream, AnakatechErrorResponse>>> GetLaunchGameUrlAsBytesAsync(
        Uri baseUrl,
        AnakatechLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        const string methodName = "launchGame";
        return PostAsync<Stream, AnakatechLaunchGameApiRequest>(
            baseUrl,
            methodName,
            apiRequest,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, AnakatechErrorResponse>>> PostAsync<TSuccess, TRequest>(
        Uri baseUrl,
        string method,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{method}");

            var requestContent = JsonConvert.SerializeObject(request);
            var jsonContent = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);
            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, AnakatechErrorResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, AnakatechErrorResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, AnakatechErrorResponse> GetHttpResultAsync<TSuccess>(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;

            if (string.IsNullOrEmpty(responseBody))
            {//TODO remove braces if simple check and failure result
                return httpResponse.Failure<TSuccess, AnakatechErrorResponse>();
            }

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            if (responseJson.ValueKind is JsonValueKind.Object
             && responseJson.TryGetProperty("error", out var error)
             && !error.ValueKind.Equals(JsonValueKind.Null))
            {
                var errorResponse = responseJson.Deserialize<AnakatechErrorResponse>(_jsonSerializerOptions);
                if (errorResponse is not null)
                    return httpResponse.Failure<TSuccess, AnakatechErrorResponse>(errorResponse);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);

            return success is null
                ? httpResponse.Failure<TSuccess, AnakatechErrorResponse>()
                : httpResponse.Success<TSuccess, AnakatechErrorResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, AnakatechErrorResponse>(e);
        }
    }
}