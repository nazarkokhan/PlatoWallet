namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;

using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Domain.Entities;
using Domain.Entities.Enums;
using DTOs.Requests;
using DTOs.Responses;
using Hub88GamesApi.DTOs.Requests;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StartupSettings.Options;

public class SoftswissGamesApiClient : ISoftswissGamesApiClient
{
    private readonly ILogger<SoftswissGamesApiClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _softswissJsonSerializerOptions;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SoftswissCurrenciesOptions _currencyMultipliers;

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
        _softswissJsonSerializerOptions = jsonOptions.Get(nameof(CasinoProvider.Softswiss)).JsonSerializerOptions;
    }

    public async Task<ISoftswissResult<SoftswissGetGameLinkGameApiResponse>> GetLaunchUrlAsync(
        string casinoId,
        string user,
        Guid sessionId,
        string game,
        string currency,
        long balance,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<SoftswissGetLaunchUrlGameApiRequest, SoftswissGetGameLinkGameApiResponse>(
            "sessions",
            new SoftswissGetLaunchUrlGameApiRequest(
                casinoId,
                game,
                currency,
                "de",
                "46.53.162.55",
                _currencyMultipliers.GetSumOut(currency, balance),
                "desktop",
                new SoftswissGamesApiUrls(
                    "https://bitstarz.com/accounts/BTC/deposit?redirected_from_game=true",
                    "https://bitstarz.com/exit_iframe"),
                "DE",
                new SoftswissGamesApiUser(
                    user,
                    user)),
            cancellationToken);

        return response;
    }

    public async Task<ISoftswissResult> IssueFreespinsAsync(
        SoftswissIssueFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync(
            "freespins/issue",
            request,
            cancellationToken);

        return response;
    }

    public async Task<ISoftswissResult> CancelFreespinsAsync(
        SoftswissCancelFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync(
            "freespins/cancel",
            request,
            cancellationToken);

        return response;
    }

    public async Task<ISoftswissResult<SoftswissRoundDetailsGameApiResponse>> RoundDetailsAsync(
        SoftswissRoundDetailsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<SoftswissRoundDetailsGameApiRequest, SoftswissRoundDetailsGameApiResponse>(
            "rounds/details",
            request,
            cancellationToken);

        return response;
    }

    private async Task<ISoftswissResult<TResponse>> PostSignedRequestAsync<TRequest, TResponse>(
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : IPswGamesApiBaseRequest
    {
        try
        {
            var jsonContent = await CreateSignedContentAsync(request, cancellationToken);

            var httpResponse = await _httpClient.PostAsync(requestUri, jsonContent, cancellationToken);

            var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var responseJsonNode = JsonNode.Parse(responseString);

            var error = responseJsonNode?["code"]?.GetValue<string>();

            if (error is not null)
                return SoftswissResultFactory.Failure<TResponse>(Enum.Parse<SoftswissErrorCode>(error));

            if (httpResponse.StatusCode is not (HttpStatusCode.OK or HttpStatusCode.Created))
                return SoftswissResultFactory.Failure<TResponse>(SoftswissErrorCode.UnknownError);

            var response = responseJsonNode.Deserialize<TResponse>(_softswissJsonSerializerOptions);

            if (response is null)
                return SoftswissResultFactory.Failure<TResponse>(SoftswissErrorCode.UnknownError);

            return SoftswissResultFactory.Success(response);
        }
        catch (Exception e)
        {
            return SoftswissResultFactory.Failure<TResponse>(SoftswissErrorCode.UnknownError, null, e);
        }
    }

    private async Task<ISoftswissResult> PostSignedRequestAsync<TRequest>(
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : IPswGamesApiBaseRequest
    {
        try
        {
            var jsonContent = await CreateSignedContentAsync(request, cancellationToken);

            var httpResponse = await _httpClient.PostAsync(requestUri, jsonContent, cancellationToken);

            var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(responseString) && httpResponse.StatusCode is HttpStatusCode.OK)
                return SoftswissResultFactory.Success();

            var responseJsonNode = JsonNode.Parse(responseString);

            var error = responseJsonNode?["code"]?.GetValue<string>();

            if (error is null)
                return SoftswissResultFactory.Failure(SoftswissErrorCode.UnknownError);

            return SoftswissResultFactory.Failure(Enum.Parse<SoftswissErrorCode>(error));
        }
        catch (Exception e)
        {
            return SoftswissResultFactory.Failure(SoftswissErrorCode.UnknownError, null, e);
        }
    }

    private async Task<JsonContent> CreateSignedContentAsync<T>(T request, CancellationToken cancellationToken)
        where T : IPswGamesApiBaseRequest
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

        var xRequestSign = SoftswissRequestSign.Compute(requestBytes, casino.SignatureKey);

        jsonContent.Headers.Add(PswHeaders.XRequestSign, xRequestSign);

        _logger.LogInformation(
            "GamesApi Request: {GamesApiRequest}, X-REQUEST-SIGN: {GamesApiRequestSign}",
            Encoding.UTF8.GetString(requestBytes),
            xRequestSign);

        return jsonContent;
    }
}