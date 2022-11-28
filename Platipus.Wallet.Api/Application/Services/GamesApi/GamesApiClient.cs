namespace Platipus.Wallet.Api.Application.Services.GamesApi;

using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Domain.Entities;
using DTOs.Responses;
using Hub88GamesApi.DTOs.Requests;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class GamesApiClient : IGamesApiClient
{
    private readonly ILogger<GamesApiClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _pswJsonSerializerOptions;
    private readonly IServiceScopeFactory _scopeFactory;

    public GamesApiClient(
        HttpClient httpClient,
        IOptionsMonitor<JsonOptions> jsonOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<GamesApiClient> logger)
    {
        _httpClient = httpClient;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _pswJsonSerializerOptions = jsonOptions.CurrentValue.JsonSerializerOptions;
    }

    public async Task<IPswResult<GetLaunchUrlResponseDto>> GetLaunchUrlAsync(
        string casinoId,
        Guid sessionId,
        string user,
        string currency,
        string game,
        string locale,
        string lobby,
        string launchMode,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<GetLaunchUrlResponseDto, PswGetGameLinkGamesApiRequest>(
            "game/session",
            new PswGetGameLinkGamesApiRequest(
                casinoId,
                sessionId,
                user,
                currency,
                game,
                locale,
                lobby,
                launchMode),
            cancellationToken);

        return response;
    }

    public async Task<IPswResult<PswGetCasinoGamesListGamesApiResponseDto>> GetCasinoGamesAsync(
        string casinoId,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<PswGetCasinoGamesListGamesApiResponseDto, PswGetCasinoGamesGamesApiRequest>(
            "game/list",
            new PswGetCasinoGamesGamesApiRequest(casinoId),
            cancellationToken);

        return response;
    }

    public async Task<IPswResult<CreateFreebetAwardResponseDto>> CreateFreebetAwardAsync(
        string casinoId,
        string user,
        string awardId,
        string currency,
        string[] games,
        DateTime validUntil,
        int count,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<CreateFreebetAwardResponseDto, PswCreateFreebetAwardGamesApiRequest>(
            "freebet/award",
            new PswCreateFreebetAwardGamesApiRequest(
                casinoId,
                user,
                awardId,
                currency,
                games,
                validUntil,
                count),
            cancellationToken);

        return response;
    }

    private async Task<IPswResult<TResponse>> PostSignedRequestAsync<TResponse, TRequest>(
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

            var error = responseJsonNode?["error"]?.GetValue<string>();

            if (error is not null)
                return PswResultFactory.Failure<TResponse>(Enum.Parse<PswErrorCode>(error));

            if (httpResponse.StatusCode is not HttpStatusCode.OK)
                return PswResultFactory.Failure<TResponse>(PswErrorCode.Unknown);

            var response = responseJsonNode.Deserialize<TResponse>(_pswJsonSerializerOptions);

            if (response is null)
                return PswResultFactory.Failure<TResponse>(PswErrorCode.Unknown);

            return PswResultFactory.Success(response);
        }
        catch (Exception e)
        {
            return PswResultFactory.Failure<TResponse>(PswErrorCode.Unknown, e);
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

        var jsonContent = JsonContent.Create(request, options: _pswJsonSerializerOptions);
        var requestBytes = await jsonContent.ReadAsByteArrayAsync(cancellationToken);

        var xRequestSign = PswRequestSign.Compute(requestBytes, casino.SignatureKey);

        jsonContent.Headers.Add(PswHeaders.XRequestSign, xRequestSign);

        _logger.LogInformation(
            "GamesApi Request: {GamesApiRequest}, X-REQUEST-SIGN: {GamesApiRequestSign}",
            Encoding.UTF8.GetString(requestBytes),
            xRequestSign);

        return jsonContent;
    }
}