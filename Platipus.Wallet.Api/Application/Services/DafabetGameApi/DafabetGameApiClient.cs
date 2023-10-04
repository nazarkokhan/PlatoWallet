namespace Platipus.Wallet.Api.Application.Services.DafabetGameApi;

using System.Text.Json;
using System.Text.RegularExpressions;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Requests;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class DafabetGameApiClient : IDafabetGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "dafabet/";

    public DafabetGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(WalletProvider.Dafabet))
           .JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<string, DafabetErrorResponse>>> GetLaunchScriptAsync(
        Uri baseUrl,
        string signatureKey,
        DafabetGetLaunchUrlGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        const string methodRoute = "launch";

        var hashValue = DatabetSecurityHash.Compute(
            methodRoute,
            $"{apiRequest.GameCode}{apiRequest.PlayerId}{apiRequest.PlayerToken}{apiRequest.Currency}",
            signatureKey);

        var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(apiRequest);
        queryParamsCollection.Add("hash", hashValue);

        return GetAsync<string>(
            baseUrl,
            methodRoute,
            queryParamsCollection,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, DafabetErrorResponse>>> GetAsync<TSuccess>(
        Uri baseUrl,
        string methodRoute,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}/{methodRoute}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, methodRoute);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, DafabetErrorResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, DafabetErrorResponse>>(ErrorCode.UnknownHttpClientError);
        }
    }

    private IHttpClientResult<TSuccess, DafabetErrorResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string methodRoute)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
            return httpResponse.Failure<TSuccess, DafabetErrorResponse>();

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            var regex = new Regex("\"status\"\\s*:\\s*0");
            if (methodRoute is "launch" && !regex.IsMatch(responseBody))
                return httpResponse.Success<TSuccess, DafabetErrorResponse>((TSuccess)(object)responseBody);

            throw;
        }

        return methodRoute switch
        {
            "launch" => HandleLaunchGameResponse<TSuccess>(parsedJson, responseBody, httpResponse),
            _ => HandleDefaultResponse<TSuccess>(parsedJson, httpResponse)
        };
    }

    private IHttpClientResult<TSuccess, DafabetErrorResponse> HandleLaunchGameResponse<TSuccess>(
        JsonDocument parsedJson,
        string responseBody,
        HttpClientRequest httpResponse)
    {
        var statusValue = parsedJson.RootElement.TryGetProperty("status", out var status) ? status.GetInt32() : 0;
        if (statusValue is not 0)
            return httpResponse.Success<TSuccess, DafabetErrorResponse>((TSuccess)(object)responseBody);

        var errorResponse = parsedJson.RootElement.Deserialize<DafabetErrorResponse>(_jsonSerializerOptions);
        return errorResponse is not null
            ? httpResponse.Failure<TSuccess, DafabetErrorResponse>(errorResponse)
            : httpResponse.Failure<TSuccess, DafabetErrorResponse>();
    }

    private IHttpClientResult<TSuccess, DafabetErrorResponse> HandleDefaultResponse<TSuccess>(
        JsonDocument parsedJson,
        HttpClientRequest httpResponse)
    {
        var root = parsedJson.RootElement;
        if (root.ValueKind == JsonValueKind.Object
         && root.TryGetProperty("status", out var status)
         && status.ValueKind is not JsonValueKind.Null
         && status.GetInt32() is 0)
        {
            var errorResponse = root.Deserialize<DafabetErrorResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, DafabetErrorResponse>(errorResponse);
        }

        var success = root.Deserialize<TSuccess>(_jsonSerializerOptions);
        return success is null
            ? httpResponse.Failure<TSuccess, DafabetErrorResponse>()
            : httpResponse.Success<TSuccess, DafabetErrorResponse>(success);
    }
}