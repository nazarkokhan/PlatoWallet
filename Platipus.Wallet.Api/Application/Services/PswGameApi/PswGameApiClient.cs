namespace Platipus.Wallet.Api.Application.Services.PswGameApi;

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Extensions.SecuritySign;
using Domain.Entities;
using Infrastructure.Persistence;
using Requests;
using Responses;

public class PswGameApiClient : IPswGameApiClient
{
    private readonly ILogger<PswGameApiClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IServiceScopeFactory _scopeFactory;

    public PswGameApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<PswGameApiClient> logger)
    {
        _httpClient = httpClient;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _jsonSerializerOptions = jsonOptions.CurrentValue.JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<PswGameSessionGameApiResponse, PswErrorGameApiResponse>>> GameSessionAsync(
        Uri baseUrl,
        PswGameSessionGameApiRequest request,
        LaunchMode launchModeType,
        bool isBetflag,
        CancellationToken cancellationToken = default)
    {
        var launchModePath = launchModeType is LaunchMode.Real ? "session" : "demo";

        if (request.LaunchMode is not "url")
            request = request with { LaunchMode = "url" };
        var response = await PostSignedRequestAsync<PswGameSessionGameApiRequest, PswGameSessionGameApiResponse>(
            baseUrl,
            $"game/{launchModePath}",
            isBetflag,
            request,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<PswGameListGameApiResponse, PswErrorGameApiResponse>>> GameListAsync(
        Uri baseUrl,
        PswGameListGameApiRequest request,
        bool isBetflag,
        CancellationToken cancellationToken = default)
    {
        var response =
            await PostSignedRequestAsync<PswGameListGameApiRequest, PswGameListGameApiResponse>(
                baseUrl,
                "game/list",
                isBetflag,
                request,
                cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<PswFreebetAwardGameApiResponse, PswErrorGameApiResponse>>> FreebetAwardAsync(
        Uri baseUrl,
        PswFreebetAwardGameApiRequest request,
        bool isBetflag,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<PswFreebetAwardGameApiRequest, PswFreebetAwardGameApiResponse>(
            baseUrl,
            "freebet/award",
            isBetflag,
            request,
            cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<PswGameBuyGameApiResponse, PswErrorGameApiResponse>>> GameBuyAsync(
        Uri baseUrl,
        PswGameBuyGameApiRequest request,
        bool isBetflag,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<PswGameBuyGameApiRequest, PswGameBuyGameApiResponse>(
            baseUrl,
            "game/buy",
            isBetflag,
            request,
            cancellationToken);

        return response;
    }

    private async Task<IResult<IHttpClientResult<TSuccess, PswErrorGameApiResponse>>>
        PostSignedRequestAsync<TRequest, TSuccess>(
            Uri baseUrl,
            string method,
            bool isBetflag,
            TRequest request,
            CancellationToken cancellationToken = default)
        where TRequest : IPswGameApiBaseRequest
    {
        try
        {
            var jsonContent = await CreateSignedContentAsync(request, cancellationToken);

            var providerPath = isBetflag ? "betflag" : "psw";
            baseUrl = new Uri(baseUrl, $"{providerPath}/{method}");

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess>(httpResponse);

            return ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, PswErrorGameApiResponse>>(
                ErrorCode.UnknownHttpClientError,
                e);
        }
    }

    private async Task<JsonContent> CreateSignedContentAsync<T>(T request, CancellationToken cancellationToken)
        where T : IPswGameApiBaseRequest
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

        var casino = await dbContext.Set<Casino>()
           .Where(c => c.Id == request.CasinoId)
           .Select(
                c => new
                {
                    c.Provider,
                    c.SignatureKey
                })
           .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            throw new NullReferenceException("Casino not found");

        var jsonContent = JsonContent.Create(request, options: _jsonSerializerOptions);
        var requestBytes = await jsonContent.ReadAsByteArrayAsync(cancellationToken);

        var xRequestSign = PswSecuritySign.Compute(requestBytes, casino.SignatureKey);
        xRequestSign = xRequestSign.ToLower();

        jsonContent.Headers.Add(PswHeaders.XRequestSign, xRequestSign);

        var rawRequestJson = Encoding.UTF8.GetString(requestBytes);
        _logger.LogInformation(
            "GamesApi Request: {GamesApiRequest}, X-REQUEST-SIGN: {GamesApiRequestSign}",
            rawRequestJson,
            xRequestSign);

        return jsonContent;
    }

    private IHttpClientResult<TSuccess, PswErrorGameApiResponse> GetHttpResultAsync<TSuccess>(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;
            if (responseBody is null)
                return httpResponse.Failure<TSuccess, PswErrorGameApiResponse>();

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            var isError = responseJson.TryGetProperty("error", out var errorCode);
            if (isError)
            {
                var error = responseJson.Deserialize<PswErrorGameApiResponse>(_jsonSerializerOptions);
                return httpResponse.Failure<TSuccess, PswErrorGameApiResponse>(error!);
            }

            var success = responseJson.Deserialize<TSuccess>(_jsonSerializerOptions);
            if (success is null)
                return httpResponse.Failure<TSuccess, PswErrorGameApiResponse>();

            return httpResponse.Success<TSuccess, PswErrorGameApiResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, PswErrorGameApiResponse>(e);
        }
    }
}