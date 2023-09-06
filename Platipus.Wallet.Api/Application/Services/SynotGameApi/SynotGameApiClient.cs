namespace Platipus.Wallet.Api.Application.Services.SynotGameApi;

using System.Text.Json;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Domain.Entities;
using Domain.Entities.Enums;
using External;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Requests;
using Responses.Synot.Base;
using Results.HttpClient;
using Results.HttpClient.HttpData;
using Results.HttpClient.WithData;
using StartupSettings.Constants.Synot;

public sealed class SynotGameApiClient : ISynotGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly WalletDbContext _walletDbContext;

    private const string ApiBasePath = "synot/";

    public SynotGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonSerializerOptions,
        WalletDbContext walletDbContext)
    {
        _httpClient = httpClient;
        _walletDbContext = walletDbContext;
        _jsonSerializerOptions = jsonSerializerOptions.Get(nameof(WalletProvider.Synot))
           .JsonSerializerOptions;
    }

    public Task<IResult<IHttpClientResult<SynotGetGamesResponse, SynotErrorResponse>>> GetGamesAsync(
        string casinoId,
        Uri baseUrl,
        CancellationToken cancellationToken = default)
    {
        var request = new Dictionary<string, string?>();

        return GetAsync<SynotGetGamesResponse>(
            baseUrl,
            "getGames",
            request,
            casinoId,
            cancellationToken);
    }

    public Task<IResult<IHttpClientResult<string, SynotErrorResponse>>> GetGameLaunchScriptAsync(
        string casinoId,
        Uri baseUrl,
        SynotGetGameLaunchScriptGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var requestToServer = new Dictionary<string, string?>
        {
            { nameof(request.Token), request.Token },
            { nameof(request.Currency), request.Currency },
            { nameof(request.LobbyUrl).ToCamelCase(), request.LobbyUrl },
            { nameof(request.Language), request.Language },
            { nameof(request.Game), request.Game },
            { nameof(request.Real), request.Real.ToString() },
        };

        return GetAsync<string>(
            baseUrl,
            "launch",
            requestToServer,
            casinoId,
            cancellationToken);
    }

    private async Task<IResult<IHttpClientResult<TSuccess, SynotErrorResponse>>> GetAsync<TSuccess>(
        Uri baseUrl,
        string methodRoute,
        Dictionary<string, string?> request,
        string casinoId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{casinoId}/{methodRoute}{QueryString.Create(request)}");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl);

            if (methodRoute is not "launch")
            {
                var casinoData = await _walletDbContext.Set<Casino>()
                   .Where(c => c.Id.Contains("synot"))
                   .Select(
                        x => new
                        {
                            ApiKey = x.Params.SynotApiKey,
                            SecretKey = x.SignatureKey
                        })
                   .FirstOrDefaultAsync(cancellationToken);

                if (casinoData is null)
                {
                    return ResultFactory.Failure<IHttpClientResult<TSuccess, SynotErrorResponse>>(
                        ErrorCode.BadParametersInTheRequest);
                }

                var xEasSignature = SynotSecurityHash.Compute(request, casinoData.SecretKey, string.Empty);

                httpRequestMessage.Headers.Add(SynotConstants.XEasApiKey, casinoData.ApiKey);
                httpRequestMessage.Headers.Add(SynotConstants.XEasSignature, xEasSignature);
            }

            var httpResponseOriginal = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse, methodRoute);

            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, SynotErrorResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, SynotErrorResponse>>(ErrorCode.UnknownHttpClientError);
        }
    }

    private IHttpClientResult<TSuccess, SynotErrorResponse> GetHttpResultAsync<TSuccess>(
        HttpClientRequest httpResponse,
        string methodRoute)
    {
        var responseBody = httpResponse.ResponseData.Body;

        if (string.IsNullOrEmpty(responseBody))
            return httpResponse.Failure<TSuccess, SynotErrorResponse>();

        JsonDocument parsedJson;
        try
        {
            parsedJson = JsonDocument.Parse(responseBody);
        }
        catch
        {
            if (methodRoute is "launch" && (!responseBody.Contains("errorCode") || !responseBody.Contains("code")))
                return httpResponse.Success<TSuccess, SynotErrorResponse>((TSuccess)(object)responseBody);

            throw;
        }

        return methodRoute switch
        {
            "launch" => HandleLaunchGameResponse<TSuccess>(parsedJson, responseBody, httpResponse),
            _ => HandleDefaultResponse<TSuccess>(parsedJson, httpResponse)
        };
    }

    private IHttpClientResult<TSuccess, SynotErrorResponse> HandleLaunchGameResponse<TSuccess>(
        JsonDocument parsedJson,
        string responseBody,
        HttpClientRequest httpResponse)
    {
        if (!parsedJson.RootElement.TryGetProperty("errorCode", out _)
         || !parsedJson.RootElement.TryGetProperty("message", out _))
            return httpResponse.Success<TSuccess, SynotErrorResponse>((TSuccess)(object)responseBody);

        var errorResponse = parsedJson.RootElement.Deserialize<SynotErrorResponse>(_jsonSerializerOptions);
        return errorResponse is not null
            ? httpResponse.Failure<TSuccess, SynotErrorResponse>(errorResponse)
            : httpResponse.Failure<TSuccess, SynotErrorResponse>();
    }

    private IHttpClientResult<TSuccess, SynotErrorResponse> HandleDefaultResponse<TSuccess>(
        JsonDocument parsedJson,
        HttpClientRequest httpResponse)
    {
        var root = parsedJson.RootElement;
        if (root.ValueKind == JsonValueKind.Object
         && root.TryGetProperty("errorCode", out var error)
         && error.ValueKind is not JsonValueKind.Null)
        {
            var errorResponse = root.Deserialize<SynotErrorResponse>(_jsonSerializerOptions);
            if (errorResponse is not null)
                return httpResponse.Failure<TSuccess, SynotErrorResponse>(errorResponse);
        }

        var success = root.Deserialize<TSuccess>(_jsonSerializerOptions);
        return success is null
            ? httpResponse.Failure<TSuccess, SynotErrorResponse>()
            : httpResponse.Success<TSuccess, SynotErrorResponse>(success);
    }
}