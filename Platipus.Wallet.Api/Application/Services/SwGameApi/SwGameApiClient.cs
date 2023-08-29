namespace Platipus.Wallet.Api.Application.Services.SwGameApi;

using System.Text.Json;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Extensions;
using Requests;
using Responses;

public class SwGameApiClient : ISwGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SwGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Sw)).JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<SwAwardGameApiResponse, SwErrorGameApiResponse>>> CreateFreespin(
        Uri baseUrl,
        SwCreateAwardGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response =
            await PostSignedRequestAsync<SwCreateAwardGameApiRequest, SwAwardGameApiResponse>(
                baseUrl,
                "FREESPIN.DO",
                request,
                cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<SwAwardGameApiResponse, SwErrorGameApiResponse>>> DeleteFreespin(
        Uri baseUrl,
        SwDeleteFreespinGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<SwDeleteFreespinGameApiRequest, SwAwardGameApiResponse>(
            baseUrl,
            "DELETEFREESPIN.DO",
            request,
            cancellationToken);

        return response;
    }

    private async Task<IResult<IHttpClientResult<TSuccess, SwErrorGameApiResponse>>>
        PostSignedRequestAsync<TRequest, TSuccess>(
            Uri baseUrl,
            string method,
            TRequest queryRequest,
            CancellationToken cancellationToken = default)
        where TRequest : class
    {
        try
        {
            var queryDictionary = ObjectToDictionaryConverter.ConvertToDictionary(queryRequest);

            var queryString = QueryString.Create(queryDictionary);
            baseUrl = new Uri(baseUrl, $"{method}{queryString}");

            var jsonContent = JsonContent.Create<object?>(null);

            var httpResponseOriginal = await _httpClient.PostAsJsonAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, SwErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private IHttpClientResult<TSuccess, SwErrorGameApiResponse> GetHttpResultAsync<TSuccess>(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, SwErrorGameApiResponse>();

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            var isError = responseJson.TryGetProperty("result", out var resultStatus);
            if (isError && resultStatus.GetString() == "ERROR")
            {
                var error = responseJson.Deserialize<SwErrorGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<TSuccess, SwErrorGameApiResponse>(error!);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            if (success is null)
                return httpResponse.Failure<TSuccess, SwErrorGameApiResponse>();

            return httpResponse.Success<TSuccess, SwErrorGameApiResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, SwErrorGameApiResponse>(e);
        }
    }
}