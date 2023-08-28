namespace Platipus.Wallet.Api.Application.Services.NemesisGamesApi;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;
using Platipus.Wallet.Api.Extensions;
using Domain.Entities.Enums;
using Responses;

public class NemesisGameApiClient : INemesisGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public NemesisGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Nemesis)).JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<string, NemesisErrorGameApiResponse>>>
        LauncherAsync(
            Uri baseUrl,
            NemesisLauncherGameApiRequest request,
            string xIntegrationToken,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParamsCollection = ObjectToDictionaryConverter.ConvertToDictionary(request);

            var orderedQueryParams = queryParamsCollection
               .OrderBy(x => x.Key)
               .Select(x => x.Key + HttpUtility.UrlEncode(x.Value));
            var stringToHash = xIntegrationToken[..4] + string.Concat(orderedQueryParams);
            var bytesToHash = Encoding.UTF8.GetBytes(stringToHash);
            var hashBytes = SHA1.HashData(bytesToHash);
            var hash = Convert.ToHexString(hashBytes).ToLower();
            queryParamsCollection.Add("qsig", hash);

            var queryString = QueryString.Create(queryParamsCollection);
            baseUrl = new Uri(baseUrl, "nemesis/launcher" + queryString);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl);

            var httpResponseOriginal = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<string, NemesisErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    public async Task<IResult<IHttpClientResult<NemesisCreateAwardGameApiResponse, NemesisErrorGameApiResponse>>>
        CreateAwardAsync(
            Uri baseUrl,
            NemesisCreateAwardGameApiRequest request,
            string xIntegrationToken,
            CancellationToken cancellationToken = default)
    {
        var response =
            await PostSignedRequestAsync<NemesisCreateAwardGameApiResponse>(
                baseUrl,
                "award",
                request,
                xIntegrationToken,
                cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<NemesisCancelAwardGameApiResponse, NemesisErrorGameApiResponse>>>
        CancelAwardAsync(
            Uri baseUrl,
            NemesisCancelAwardGameApiRequest request,
            string xIntegrationToken,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<NemesisCancelAwardGameApiResponse>(
            baseUrl,
            "cancel",
            request,
            xIntegrationToken,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<NemesisCurrenciesGameApiResponse[], NemesisErrorGameApiResponse>>>
        Currencies(
            Uri baseUrl,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<NemesisCurrenciesGameApiResponse[]>(
            baseUrl,
            "currencies",
            null,
            null,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<NemesisRoundGameApiResponse, NemesisErrorGameApiResponse>>> Round(
        Uri baseUrl,
        NemesisRoundGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<NemesisRoundGameApiResponse>(
            baseUrl,
            "round",
            request,
            null,
            cancellationToken);

        return response;
    }

    private async Task<IResult<IHttpClientResult<TSuccess, NemesisErrorGameApiResponse>>> PostSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        string method,
        object? request,
        string? xIntegrationToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonContent = JsonContent.Create(request, options: _jsonSerializerOptions);

            if (xIntegrationToken is not null)
                jsonContent.Headers.Add(NemesisHeaders.XIntegrationToken, xIntegrationToken);

            baseUrl = new Uri(baseUrl, $"nemesis/{method}");

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, NemesisErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, NemesisErrorGameApiResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, NemesisErrorGameApiResponse>();

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            var isError = responseJson.ValueKind is JsonValueKind.Object
                       && responseJson.TryGetProperty("error", out _);
            if (isError)
            {
                var error = responseJson.Deserialize<NemesisErrorGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<TSuccess, NemesisErrorGameApiResponse>(error!);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            if (success is null)
                return httpResponse.Failure<TSuccess, NemesisErrorGameApiResponse>();

            return httpResponse.Success<TSuccess, NemesisErrorGameApiResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, NemesisErrorGameApiResponse>(e);
        }
    }

    private IHttpClientResult<string, NemesisErrorGameApiResponse> GetHttpResultAsync(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<string, NemesisErrorGameApiResponse>();

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
                var error = responseJson!.Value.Deserialize<NemesisErrorGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<string, NemesisErrorGameApiResponse>(error!);
            }

            return httpResponse.Success<string, NemesisErrorGameApiResponse>(responseBody);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<string, NemesisErrorGameApiResponse>(e);
        }
    }
}