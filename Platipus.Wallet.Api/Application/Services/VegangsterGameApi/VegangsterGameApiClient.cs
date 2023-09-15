namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi;

using System.Text.Json;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Domain.Entities.Enums;
using External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Responses.Vegangster.Base;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public sealed class VegangsterGameApiClient : IVegangsterGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string ApiBasePath = "vegangster/";

    public VegangsterGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(WalletProvider.Vegangster))
           .JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<VegangsterGetLaunchUrlResponse, VegangsterFailureResponse>>> GetLaunchUrlAsync(
        Uri baseUrl,
        string casinoId,
        string xApiSignature,
        IVegangsterCommonGetLaunchUrlApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        const string methodRoute = "game/url";
        return await PostSignedRequestAsync<VegangsterGetLaunchUrlResponse>(
            baseUrl,
            casinoId,
            xApiSignature,
            methodRoute,
            (VegangsterGetLaunchUrlGameApiRequest)apiRequest,
            cancellationToken);
    }

    public async Task<IResult<IHttpClientResult<VegangsterGetLaunchUrlResponse, VegangsterFailureResponse>>>
        GetDemoLaunchUrlAsync(
            Uri baseUrl,
            string casinoId,
            string xApiSignature,
            IVegangsterCommonGetLaunchUrlApiRequest apiRequest,
            CancellationToken cancellationToken = default)
    {
        const string methodRoute = "game/demo/url";
        return await PostSignedRequestAsync<VegangsterGetLaunchUrlResponse>(
            baseUrl,
            casinoId,
            xApiSignature,
            methodRoute,
            (VegangsterGetDemoLaunchUrlGameApiRequest)apiRequest,
            cancellationToken);
    }

    public Task<IResult<IHttpClientResult<VegangsterGetAvailableGamesResponse, VegangsterFailureResponse>>>
        GetAvailableGamesAsync(
            Uri baseUrl,
            string casinoId,
            string xApiSignature,
            IVegangsterCommonGetLaunchUrlApiRequest apiRequest,
            CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<IHttpClientResult<VegangsterGetAvailableGamesResponse, VegangsterFailureResponse>>>
        GetAvailableGamesAsync(
            Uri baseUrl,
            string casinoId,
            string xApiSignature,
            VegangsterGetAvailableGamesGameApiRequest apiRequest,
            CancellationToken cancellationToken = default)
    {
        const string methodRoute = "game/list";
        return await PostSignedRequestAsync<VegangsterGetAvailableGamesResponse>(
            baseUrl,
            casinoId,
            xApiSignature,
            methodRoute,
            apiRequest,
            cancellationToken);
    }

    public async Task<IResult<IHttpClientResult<VegangsterGrantResponse, VegangsterFailureResponse>>> GrantAsync(
        Uri baseUrl,
        string casinoId,
        string xApiSignature,
        VegangsterGrantGameApiRequest apiRequest,
        CancellationToken cancellationToken = default)
    {
        const string methodRoute = "freegames/grant";
        return await PostSignedRequestAsync<VegangsterGrantResponse>(
            baseUrl,
            casinoId,
            xApiSignature,
            methodRoute,
            apiRequest,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, VegangsterFailureResponse>>> PostSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        string vegangsterSignatureKey,
        string casinoId,
        string methodRoute,
        object request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{casinoId}{methodRoute}");

            var jsonContent = JsonContent.Create(request, options: _jsonSerializerOptions);
            var requestBytes = await jsonContent.ReadAsByteArrayAsync(cancellationToken);
            var xApiSignature = Hub88SecuritySign.Compute(requestBytes, vegangsterSignatureKey);

            jsonContent.Headers.Add(VegangsterHeaders.XApiSignature, xApiSignature);

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);
            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, VegangsterFailureResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, VegangsterFailureResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, VegangsterFailureResponse> GetHttpResultAsync<TSuccess>(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;

            if (string.IsNullOrEmpty(responseBody))
            {
                return httpResponse.Failure<TSuccess, VegangsterFailureResponse>();
            }

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            if (responseJson.TryGetProperty("error", out var error) && !error.ValueKind.Equals(JsonValueKind.Null))
            {
                var errorResponse = responseJson.Deserialize<VegangsterFailureResponse>(_jsonSerializerOptions);
                if (errorResponse is not null)
                    return httpResponse.Failure<TSuccess, VegangsterFailureResponse>(errorResponse);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);

            return success is null
                ? httpResponse.Failure<TSuccess, VegangsterFailureResponse>()
                : httpResponse.Success<TSuccess, VegangsterFailureResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, VegangsterFailureResponse>(e);
        }
    }
}