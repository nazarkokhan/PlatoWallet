namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;

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

    public async Task<IResult<SoftswissBoxGamesApiResponse<SoftswissGetGameLinkGameApiResponse>>> GetLaunchUrlAsync(
        Uri baseUrl,
        string casinoId,
        string user,
        string sessionId,
        int gameId,
        string currency,
        long balance,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<SoftswissGetLaunchUrlGameApiRequest, SoftswissGetGameLinkGameApiResponse>(
            "softswiss/sessions",
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

    public async Task<IResult<SoftswissBoxGamesApiResponse<string>>> IssueFreespinsAsync(
        Uri baseUrl,
        SoftswissIssueFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<SoftswissIssueFreespinsGameApiRequest, string>(
            "softswiss/freespins/issue",
            baseUrl,
            request,
            cancellationToken);

        return response;
    }

    public async Task<IResult<SoftswissBoxGamesApiResponse<SoftswissCancelFreespinsGameApiResponse>>> CancelFreespinsAsync(
        Uri baseUrl,
        SoftswissCancelFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response
            = await PostSignedRequestAsync<SoftswissCancelFreespinsGameApiRequest, SoftswissCancelFreespinsGameApiResponse>(
                "softswiss/freespins/cancel",
                baseUrl,
                request,
                cancellationToken);

        return response;
    }

    public async Task<IResult<SoftswissBoxGamesApiResponse<SoftswissRoundDetailsGameApiResponse>>> RoundDetailsAsync(
        Uri baseUrl,
        SoftswissRoundDetailsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<SoftswissRoundDetailsGameApiRequest, SoftswissRoundDetailsGameApiResponse>(
            "softswiss/rounds/details",
            baseUrl,
            request,
            cancellationToken);

        return response;
    }

    private async Task<IResult<SoftswissBoxGamesApiResponse<TResponse>>> PostSignedRequestAsync<TRequest, TResponse>(
        string requestUri,
        Uri baseUrl,
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : IPswGamesApiBaseRequest where TResponse : class
    {
        try
        {
            var jsonContent = await CreateSignedContentAsync(request, cancellationToken);

            var httpResponse = await _httpClient.PostAsync(new Uri(baseUrl, requestUri), jsonContent, cancellationToken);
            var httpRequest = httpResponse.RequestMessage!;
            var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            var httpRequestMessage = $"{httpRequest!}\nBody:\n{await httpRequest.Content!.ReadAsStringAsync(cancellationToken)}";
            var httpResponseMessage = $"{httpResponse}\nBody:\n{responseString}";
            var response = new SoftswissBoxGamesApiResponse<TResponse>(
                httpRequestMessage,
                httpResponseMessage);

            if (typeof(TResponse) == typeof(string))
            {
                response.Content = responseString as TResponse;
                return ResultFactory.Success(response);
            }

            var responseJsonNode = JsonNode.Parse(responseString);
            var error = responseJsonNode?["code"]?.GetValue<string>();

            if (error is not null)
                return ResultFactory.Success(response);

            var content = responseJsonNode.Deserialize<TResponse>(_softswissJsonSerializerOptions);
            response.Content = content;

            return ResultFactory.Success(response);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<SoftswissBoxGamesApiResponse<TResponse>>(ErrorCode.Unknown, e);
        }
    }

    // private async Task<IResult<SoftswissBoxGamesApiResponse<string>>> PostSignedRequestAsync<TRequest>(
    //     string requestUri,
    //     Uri baseUrl,
    //     TRequest request,
    //     CancellationToken cancellationToken)
    //     where TRequest : IPswGamesApiBaseRequest
    // {
    //     try
    //     {
    //         var jsonContent = await CreateSignedContentAsync(request, cancellationToken);
    //
    //         var httpResponse = await _httpClient.PostAsync(new Uri(baseUrl, requestUri), jsonContent, cancellationToken);
    //         var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
    //
    //         var httpRequestMessage = httpResponse.RequestMessage!.ToString();
    //         var httpResponseMessage = $"{httpResponse}\n{responseString}";
    //         var response = new SoftswissBoxGamesApiResponse<string>(
    //             httpRequestMessage,
    //             httpResponseMessage) { Content = responseString };
    //
    //         return ResultFactory.Success(response);
    //     }
    //     catch (Exception e)
    //     {
    //         return ResultFactory.Failure<SoftswissBoxGamesApiResponse<string>>(ErrorCode.Unknown, e);
    //     }
    // }

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

        var xRequestSign = SoftswissSecurityHash.Compute(requestBytes, casino.SignatureKey);

        jsonContent.Headers.Add(PswHeaders.XRequestSign, xRequestSign);

        _logger.LogInformation(
            "GamesApi Request: {GamesApiRequest}, X-REQUEST-SIGN: {GamesApiRequestSign}",
            Encoding.UTF8.GetString(requestBytes),
            xRequestSign);

        return jsonContent;
    }
}

// public record SoftswissErrorGameApiResponse(int Code, string Message);

public record SoftswissBoxGamesApiResponse<TContent>(
    // TContent? Content,
    string HttpRequest,
    string HttpResponse)
{
    public TContent? Content { get; set; }
}

public record SoftswissCancelFreespinsGameApiResponse(string CasinoId, string IssueId);