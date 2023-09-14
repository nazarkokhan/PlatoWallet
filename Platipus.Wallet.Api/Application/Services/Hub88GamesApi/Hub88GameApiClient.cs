namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi;

using System.Text.Json;
using DTOs.Requests;
using DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Results.Hub88;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Extensions.SecuritySign;
using Domain.Entities.Enums;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;

public class Hub88GameApiClient : IHub88GameApiClient
{
    private const string Hub88 = "hub88/";
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _hub88JsonSerializerOptions;

    public Hub88GameApiClient(HttpClient httpClient, IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _hub88JsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Hub88)).JsonSerializerOptions;
    }

    //Game
    public async Task<IResult<IHttpClientResult<Hub88GameUrlGameApiResponse, Hub88ErrorGameApiResponse>>> GetLaunchUrlAsync(
        Uri baseUrl,
        string xHub88Signature,
        Hub88GetLaunchUrlGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88GameUrlGameApiResponse>(
            baseUrl,
            $"{Hub88}game/url",
            xHub88Signature,
            request,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<Hub88CreateAwardGameApiResponse, Hub88ErrorGameApiResponse>>>
        CreateAwardAsync(
            Uri baseUrl,
            string xHub88Signature,
            Hub88CreateAwardGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88CreateAwardGameApiResponse>(
            baseUrl,
            $"{Hub88}rewards/create",
            xHub88Signature,
            request,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<Hub88DeleteAwardGameApiResponse, Hub88ErrorGameApiResponse>>>
        DeleteAwardAsync(
            Uri baseUrl,
            string xHub88Signature,
            Hub88DeleteAwardGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88DeleteAwardGameApiResponse>(
            baseUrl,
            $"{Hub88}rewards/cancel",
            xHub88Signature,
            request,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<List<Hub88PrepaidGameApiResponseItem>, Hub88ErrorGameApiResponse>>>
        GetPrepaidsListAsync(
            Uri baseUrl,
            string xHub88Signature,
            Hub88GetPrepaidsListGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<List<Hub88PrepaidGameApiResponseItem>>(
            baseUrl,
            $"{Hub88}prepaids/list",
            xHub88Signature,
            request,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<List<Hub88GameGameApiResponseItem>, Hub88ErrorGameApiResponse>>>
        GetGameListAsync(
            Uri baseUrl,
            string xHub88Signature,
            Hub88GetGameListGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<List<Hub88GameGameApiResponseItem>>(
            baseUrl,
            $"{Hub88}/game/list",
            xHub88Signature,
            request,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<Hub88GetRoundGameApiResponse, Hub88ErrorGameApiResponse>>> GetGameRoundAsync(
        Uri baseUrl,
        string xHub88Signature,
        Hub88GetRoundGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88GetRoundGameApiResponse>(
            baseUrl,
            $"{Hub88}/game/round",
            xHub88Signature,
            request,
            cancellationToken);

        return response;
    }


    private async Task<IResult<IHttpClientResult<TSuccess, Hub88ErrorGameApiResponse>>> PostSignedRequestAsync<TSuccess>(
        Uri baseUrl,
        string method,
        string xHub88Signature,
        object request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonContent = JsonContent.Create(request, options: _hub88JsonSerializerOptions);
            var requestBytes = await jsonContent.ReadAsByteArrayAsync(cancellationToken);
            var xRequestSign = Hub88SecuritySign.Compute(requestBytes, xHub88Signature);

            jsonContent.Headers.Add(Hub88Headers.XHub88Signature, xRequestSign);
            baseUrl = new Uri(baseUrl, method);

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, Hub88ErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, Hub88ErrorGameApiResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, Hub88ErrorGameApiResponse>();

            var responseJson = JsonDocument.Parse(responseBody).RootElement;
            JsonSerializer.Deserialize<TSuccess>(responseBody, _hub88JsonSerializerOptions);
            if (responseJson.ValueKind is JsonValueKind.Object)
            {
                var isError = responseJson.TryGetProperty("error", out var successStatus);
                if (isError && successStatus.GetString() != nameof(Hub88ErrorCode.RS_OK))
                {
                    var error = responseJson.Deserialize<Hub88ErrorGameApiResponse>(_hub88JsonSerializerOptions);
                    return httpResponse.Failure<TSuccess, Hub88ErrorGameApiResponse>(error!);
                }
            }

            var success = responseJson.Deserialize<TSuccess>(_hub88JsonSerializerOptions);
            if (success is null)
                return httpResponse.Failure<TSuccess, Hub88ErrorGameApiResponse>();

            return httpResponse.Success<TSuccess, Hub88ErrorGameApiResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, Hub88ErrorGameApiResponse>(e);
        }
    }
}