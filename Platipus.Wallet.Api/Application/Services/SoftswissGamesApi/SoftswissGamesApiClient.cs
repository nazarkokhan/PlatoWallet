namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.HttpClient;
using Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;
using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using DTOs;
using Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;
using Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Responses;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Extensions.SecuritySign;
using StartupSettings.Options;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;

public sealed class SoftswissGamesApiClient : ISoftswissGamesApiClient
{
    private readonly ILogger<SoftswissGamesApiClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _softswissJsonSerializerOptions;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SoftswissCurrenciesOptions _currencyMultipliers;
    private const string ApiBasePath = "softswiss/";

    public SoftswissGamesApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<SoftswissGamesApiClient> logger,
        IOptions<SoftswissCurrenciesOptions> currencyMultipliers)
    {
        _httpClient = httpClient;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _currencyMultipliers = currencyMultipliers.Value;
        _softswissJsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Softswiss)).JsonSerializerOptions;
    }

    public async Task<IResult<IHttpClientResult<SoftswissGetGameLinkGameApiResponse, SoftswissGetGameLinkGameApiResponse>>>
        GetLaunchUrlAsync(
            Uri baseUrl,
            string casinoId,
            string user,
            int gameId,
            string currency,
            long balance,
            CancellationToken cancellationToken = default)
    {
        var response =
            await PostSignedRequestAsync<SoftswissGetGameLinkGameApiResponse, SoftswissGetGameLinkGameApiResponse,
                SoftswissGetLaunchUrlGameApiRequest>(
                "sessions",
                baseUrl,
                new SoftswissGetLaunchUrlGameApiRequest(
                    casinoId,
                    gameId,
                    currency,
                    "en",
                    "46.53.162.55",
                    _currencyMultipliers.GetSumOut(currency, balance),
                    "desktop",
                    new SoftswissGamesApiUrls(
                        "https://bitstarz.com/accounts/BTC/deposit?redirected_from_game=true",
                        "https://bitstarz.com/exit_iframe"),
                    // "DE",
                    new SoftswissGamesApiUser(
                        user,
                        user)),
                cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<string, string>>> IssueFreespinsAsync(
        Uri baseUrl,
        SoftswissIssueFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<string, string, SoftswissIssueFreespinsGameApiRequest>(
            "freespins/issue",
            baseUrl,
            request,
            cancellationToken);

        return response;
    }

    public async Task<
            IResult<IHttpClientResult<SoftswissCancelFreespinsGameApiResponse, SoftswissCancelFreespinsGameApiResponse>>>
        CancelFreespinsAsync(
            Uri baseUrl,
            SoftswissCancelFreespinsGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response
            = await PostSignedRequestAsync<SoftswissCancelFreespinsGameApiResponse, SoftswissCancelFreespinsGameApiResponse,
                SoftswissCancelFreespinsGameApiRequest>(
                "freespins/cancel",
                baseUrl,
                request,
                cancellationToken);

        return response;
    }

    public async Task<IResult<IHttpClientResult<SoftswissRoundDetailsGameApiResponse, SoftswissRoundDetailsGameApiResponse>>>
        RoundDetailsAsync(
            Uri baseUrl,
            SoftswissRoundDetailsGameApiRequest request,
            CancellationToken cancellationToken = default)
    {
        var response =
            await PostSignedRequestAsync<SoftswissRoundDetailsGameApiResponse, SoftswissRoundDetailsGameApiResponse,
                SoftswissRoundDetailsGameApiRequest>(
                "rounds/details",
                baseUrl,
                request,
                cancellationToken);

        return response;
    }

    private async Task<IResult<IHttpClientResult<TSuccess, TResponse>>> PostSignedRequestAsync<TSuccess, TResponse, TRequest>(
        string methodRoute,
        Uri baseUrl,
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : ISoftswissGameApiBaseRequest
        where TResponse : class
    {
        try
        {
            baseUrl = new Uri(baseUrl, $"{ApiBasePath}{methodRoute}");
            var jsonContent = await CreateSignedContentAsync(request, cancellationToken);

            var httpResponseOriginal = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var httpResponse = await httpResponseOriginal.MapToHttpClientResponseAsync(cancellationToken);

            var httpResult = GetHttpResultAsync<TSuccess, TResponse>(httpResponse);
            return httpResult.IsFailure
                ? ResultFactory.Failure<IHttpClientResult<TSuccess, TResponse>>(ErrorCode.Unknown)
                : ResultFactory.Success(httpResult);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<IHttpClientResult<TSuccess, TResponse>>(ErrorCode.Unknown, e);
        }
    }

    private IHttpClientResult<TSuccess, TResponse> GetHttpResultAsync<TSuccess, TResponse>(HttpClientRequest httpResponse)
    {
        try
        {
            var responseBody = httpResponse.ResponseData.Body;

            if (string.IsNullOrEmpty(responseBody))
            {
                return httpResponse.Failure<TSuccess, TResponse>();
            }

            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            if (responseJson.TryGetProperty("error", out var error) && !error.ValueKind.Equals(JsonValueKind.Null))
            {
                var errorResponse = responseJson.Deserialize<TResponse>(_softswissJsonSerializerOptions);
                if (errorResponse is not null)
                    return httpResponse.Failure<TSuccess, TResponse>(errorResponse);
            }

            var success = responseJson.Deserialize<TSuccess>(_softswissJsonSerializerOptions);

            return success is null
                ? httpResponse.Failure<TSuccess, TResponse>()
                : httpResponse.Success<TSuccess, TResponse>(success);
        }
        catch (Exception e)
        {
            return httpResponse.Failure<TSuccess, TResponse>(e);
        }
    }

    private async Task<JsonContent> CreateSignedContentAsync<T>(T request, CancellationToken cancellationToken)
        where T : ISoftswissGameApiBaseRequest
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

        var jsonContent = JsonContent.Create(request, options: _softswissJsonSerializerOptions);
        var requestBytes = await jsonContent.ReadAsByteArrayAsync(cancellationToken);

        var xRequestSign = SoftswissSecurityHash.Compute(requestBytes, casino.SignatureKey);

        jsonContent.Headers.Add(PswHeaders.XRequestSign, xRequestSign);

        _logger.LogInformation(
            "GamesApi Request: {GamesApiRequest}, X-REQUEST-SIGN: {GamesApiRequestSign}",
            Encoding.UTF8.GetString(requestBytes),
            xRequestSign);

        return jsonContent;
    }
}

public sealed record SoftswissCancelFreespinsGameApiResponse(string CasinoId, string IssueId);