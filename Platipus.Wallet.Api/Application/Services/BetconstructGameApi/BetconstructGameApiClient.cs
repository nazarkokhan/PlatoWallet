namespace Platipus.Wallet.Api.Application.Services.BetconstructGameApi;

using System.Text.Json;
using System.Text.RegularExpressions;
using Api.Extensions;
using Application.Requests.Wallets.BetConstruct.Base.Response;
using Domain.Entities.Enums;
using External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class BetconstructGameApiClient : IBetconstructGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "betconstruct";

    public BetconstructGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.BetConstruct)).JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<string, BetconstructErrorResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        BetconstructGetLaunchScriptGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(apiRequest);

        var collectionWithoutNulls =
            queryParamsCollection.Where(kvp => kvp.Value is not null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return GetSignedRequestAsync<string>(
            baseUrl,
            "launch",
            collectionWithoutNulls,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, BetconstructErrorResponse>>> GetSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        string method,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var methodRoute = string.Equals("launch", method) ? string.Empty : method;
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{methodRoute}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, method);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, BetconstructErrorResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, BetconstructErrorResponse>>(
                ErrorCode.UnknownHttpClientError);
        }
    }

    private IHttpClientResult<TSuccess, BetconstructErrorResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string method)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
            return httpResponse.Failure<TSuccess, BetconstructErrorResponse>();

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            var regex = new Regex("\"Result\"\\s*:\\s*false");
            if (method is "launch" && !regex.IsMatch(responseBody))
                return httpResponse.Success<TSuccess, BetconstructErrorResponse>((TSuccess)(object)responseBody);

            throw;
        }

        return method switch
        {
            "launch" => HandleLaunchGameResponse<TSuccess>(parsedJson, responseBody, httpResponse),
            _ => HandleDefaultResponse<TSuccess>(parsedJson, httpResponse)
        };
    }

    private IHttpClientResult<TSuccess, BetconstructErrorResponse> HandleLaunchGameResponse<TSuccess>(
        JsonDocument parsedJson,
        string responseBody,
        HttpClientRequest httpResponse)
    {
        // Get the "Result" property from the JSON.
        var isSuccess = parsedJson.RootElement.TryGetProperty("Result", out var result) && result.GetBoolean();
        if (isSuccess)
            return httpResponse.Success<TSuccess, BetconstructErrorResponse>((TSuccess)(object)responseBody);

        var errorResponse = parsedJson.RootElement.Deserialize<BetconstructErrorResponse>(_jsonSerializerOptions);
        return errorResponse is not null
            ? httpResponse.Failure<TSuccess, BetconstructErrorResponse>(errorResponse)
            : httpResponse.Failure<TSuccess, BetconstructErrorResponse>();
    }

    private IHttpClientResult<TSuccess, BetconstructErrorResponse> HandleDefaultResponse<TSuccess>(
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
            var errorResponse = root.Deserialize<BetconstructErrorResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, BetconstructErrorResponse>(errorResponse);
        }

        var responseSuccess = root.Deserialize<TSuccess>(_jsonSerializerOptions);
        return responseSuccess is null
            ? httpResponse.Failure<TSuccess, BetconstructErrorResponse>()
            : httpResponse.Success<TSuccess, BetconstructErrorResponse>(responseSuccess);
    }
}