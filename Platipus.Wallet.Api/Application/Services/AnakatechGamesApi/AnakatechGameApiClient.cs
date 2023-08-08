namespace Platipus.Wallet.Api.Application.Services.AnakatechGamesApi;

using System.Text;
using System.Text.Json;
using Api.Extensions;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Requests;
using Responses.Anakatech.Base;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(WalletProvider.Anakatech))
           .JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<string, AnakatechErrorResponse>>> GetLaunchGameUrlAsBytesAsync(
        Uri baseUrl,
        AnakatechLaunchGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        const string methodName = "launchGame";
        const string lobbyUrlKey = "lobbyURL";

        var requestToSend = new Dictionary<string, string?>
        {
            { nameof(apiRequest.CustomerId).ToCamelCase(), apiRequest.CustomerId },
            { nameof(apiRequest.BrandId).ToCamelCase(), apiRequest.BrandId },
            { nameof(apiRequest.PlayerId).ToCamelCase(), apiRequest.PlayerId },
            { nameof(apiRequest.Nickname).ToCamelCase(), apiRequest.Nickname },
            { nameof(apiRequest.Currency).ToCamelCase(), apiRequest.Currency },
            { nameof(apiRequest.Language).ToCamelCase(), apiRequest.Language },
            { nameof(apiRequest.Country).ToCamelCase(), apiRequest.Country },
            { nameof(apiRequest.ProviderGameId).ToCamelCase(), apiRequest.ProviderGameId },
            { lobbyUrlKey, apiRequest.LobbyUrl },
            { nameof(apiRequest.Jurisdiction).ToCamelCase(), apiRequest.Jurisdiction },
            { nameof(apiRequest.OriginUrl).ToCamelCase(), apiRequest.OriginUrl },
            { nameof(apiRequest.PlayMode).ToCamelCase(), apiRequest.PlayMode.ToString() },
            { nameof(apiRequest.SecurityToken).ToCamelCase(), apiRequest.SecurityToken },
            { nameof(apiRequest.Balance).ToCamelCase(), apiRequest.Balance.ToString() },
            { nameof(apiRequest.RealityCheckInterval).ToCamelCase(), apiRequest.RealityCheckInterval.ToString() }
        };

        if (apiRequest.Audio is not null)
        {
            requestToSend.Add(nameof(apiRequest.Audio).ToCamelCase(), apiRequest.Audio.ToString());
        }

        if (apiRequest.RealityCheckStartTime is not null)
        {
            requestToSend.Add(
                nameof(apiRequest.RealityCheckStartTime).ToCamelCase(),
                apiRequest.RealityCheckStartTime.ToString());
        }

        if (apiRequest.MinBet is not null)
        {
            requestToSend.Add(nameof(apiRequest.MinBet).ToCamelCase(), apiRequest.MinBet.ToString());
        }

        if (apiRequest.MaxTotalBet is not null)
        {
            requestToSend.Add(nameof(apiRequest.MaxTotalBet).ToCamelCase(), apiRequest.MaxTotalBet.ToString());
        }

        return GetAsync<string>(
            baseUrl,
            methodName,
            requestToSend,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, AnakatechErrorResponse>>> GetAsync<TSuccess>(
        Uri baseUrl,
        string method,
        Dictionary<string, string?> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}/{method}{QueryString.Create(request)}");

            var httpResponseOriginal = await _httpClient.GetAsync(baseUrl, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, method);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, AnakatechErrorResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, AnakatechErrorResponse>>(ErrorCode.UnknownHttpClientError);
        }
    }

    private IHttpClientResult<TSuccess, AnakatechErrorResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string methodName)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;

            if (string.IsNullOrEmpty(responseBody))
            {
                return httpResponse.Failure<TSuccess, AnakatechErrorResponse>();
            }

            if (methodName is "launchGame")
            {
                return httpResponse.Success<TSuccess, AnakatechErrorResponse>((TSuccess)(object)responseBody);
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