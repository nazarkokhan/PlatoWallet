using System.Text.Json;
using System.Text.RegularExpressions;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Application.Services.SweepiumGameApi.Requests;
using Platipus.Wallet.Api.Extensions;

namespace Platipus.Wallet.Api.Application.Services.SweepiumGameApi;

public sealed class SweepiumGameApiClient : ISweepiumGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    
    private const string ApiBasePath = "sweepium";

    public SweepiumGameApiClient(
        HttpClient httpClient,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<string, SweepiumErrorResponse>>> LaunchAsync(
        Uri baseUrl, 
        SweepiumGetLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(apiRequest);

        var collectionWithoutNulls =
            queryParamsCollection.Where(kvp => kvp.Value is not null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return GetSignedRequestAsync<string>(
            baseUrl,
            "game/launch",
            collectionWithoutNulls,
            cancellationToken);
    }
    
    private async Task<IResult<IHttpClientResult<TSuccess, SweepiumErrorResponse>>> GetSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        string method,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var methodRoute = string.Equals("game/launch", method) ? string.Empty : method;
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{methodRoute}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, method);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, SweepiumErrorResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, SweepiumErrorResponse>>(
                ErrorCode.UnknownHttpClientError);
        }
    }
    
    private IHttpClientResult<TSuccess, SweepiumErrorResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string method)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
            return httpResponse.Failure<TSuccess, SweepiumErrorResponse>();

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            var regex = new Regex("\"Result\"\\s*:\\s*false");
            if (method is "game/launch" && !regex.IsMatch(responseBody))
                return httpResponse.Success<TSuccess, SweepiumErrorResponse>((TSuccess)(object)responseBody);

            throw;
        }

        return method switch
        {
            "game/launch" => HandleLaunchGameResponse<TSuccess>(parsedJson, responseBody, httpResponse),
            _ => HandleDefaultResponse<TSuccess>(parsedJson, httpResponse)
        };
    }
    
    private IHttpClientResult<TSuccess, SweepiumErrorResponse> HandleLaunchGameResponse<TSuccess>(
        JsonDocument parsedJson,
        string responseBody,
        HttpClientRequest httpResponse)
    {
        // Get the "Result" property from the JSON.
        var isSuccess = parsedJson.RootElement.TryGetProperty("Result", out var result) && result.GetBoolean();
        if (isSuccess)
            return httpResponse.Success<TSuccess, SweepiumErrorResponse>((TSuccess)(object)responseBody);

        var errorResponse = parsedJson.RootElement.Deserialize<SweepiumErrorResponse>(_jsonSerializerOptions);
        return errorResponse is not null
            ? httpResponse.Failure<TSuccess, SweepiumErrorResponse>(errorResponse)
            : httpResponse.Failure<TSuccess, SweepiumErrorResponse>();
    }
    
    private IHttpClientResult<TSuccess, SweepiumErrorResponse> HandleDefaultResponse<TSuccess>(
        JsonDocument parsedJson,
        HttpClientRequest httpResponse)
    {
        var root = parsedJson.RootElement;

        // Get the "Result" property from the JSON.
        if (root.ValueKind == JsonValueKind.Object
            && root.TryGetProperty("Result", out var result)
            && result.ValueKind is not JsonValueKind.Null
            && !result.GetBoolean())
        {
            var errorResponse = root.Deserialize<SweepiumErrorResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, SweepiumErrorResponse>(errorResponse);
        }

        var responseSuccess = root.Deserialize<TSuccess>(_jsonSerializerOptions);
        return responseSuccess is null
            ? httpResponse.Failure<TSuccess, SweepiumErrorResponse>()
            : httpResponse.Success<TSuccess, SweepiumErrorResponse>(responseSuccess);
    }
}