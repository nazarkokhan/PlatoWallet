namespace Platipus.Wallet.Api.Application.Services.GamesApi;

using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Domain.Entities;
using Domain.Entities.Enums;
using DTOs.Base;
using DTOs.Responses;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Requests.Wallets.Hub88.Base;
using Results.Hub88;
using Results.Hub88.WithData;
using Results.Psw;

public record GetHub88GameLinkRequestDto(
    string User,
    string Token,
    string SubPartnerId,
    string Platform,
    string OperatorId,
    Hub88GameServerMetaDto Meta,
    string LobbyUrl,
    string Lang,
    string Ip,
    int GameId,
    string GameCode,
    string DepositUrl,
    string Currency,
    string Country);

public class GamesApiClient : IGamesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _pswJsonSerializerOptions;
    private readonly JsonSerializerOptions _hub88JsonSerializerOptions;
    private readonly WalletDbContext _context;

    public GamesApiClient(HttpClient httpClient, IOptionsMonitor<JsonOptions> jsonOptions, WalletDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
        _pswJsonSerializerOptions = jsonOptions.Get(nameof(CasinoProvider.Psw)).JsonSerializerOptions;
        _hub88JsonSerializerOptions = jsonOptions.Get(nameof(CasinoProvider.Hub88)).JsonSerializerOptions;
    }

    public async Task<IHub88Result<GetHub88LaunchUrlResponseDto>> GetHub88GameLinkAsync(
        GetHub88GameLinkRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var casino = await _context.Set<Casino>()
            .Where(c => c.Id == request.SubPartnerId)
            .Select(
                c => new
                {
                    c.Provider,
                    c.SignatureKey
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return Hub88ResultFactory.Failure<GetHub88LaunchUrlResponseDto>(Hub88ErrorCode.RS_ERROR_UNKNOWN);

        var jsonContent = JsonContent.Create(request, options: _hub88JsonSerializerOptions);
        var requestBytes = await jsonContent.ReadAsByteArrayAsync(cancellationToken);

        var xRequestSign = Hub88RequestSign.Compute(requestBytes, Hub88RequestSign.KeyForGameServer);

        var pubKey = @"-----BEGIN PUBLIC KEY-----
MIGeMA0GCSqGSIb3DQEBAQUAA4GMADCBiAKBgHi57tRMYFBfHa8ZN5NTTSsK/iOK
UBmOjhzZKrrZiLjraL/U9edzftNi5KaSoXOXLpiEOvaTD+fuuXGvDbME4+XBlfav
FX8zza9FDLmERh9uhe+OLgwPu4AebHvwt8uMY2Eg5+EOc4m0uvlEDI09U54WaxgN
w9k4n3mnHboXXVHxAgMBAAE=
-----END PUBLIC KEY-----";

        var isValid = Hub88RequestSign.IsValidSign(xRequestSign, requestBytes, pubKey);

        jsonContent.Headers.Add(Hub88Headers.XHub88Signature, xRequestSign);

        var httpResponse = await _httpClient.PostAsync("hub88/game/url", jsonContent, cancellationToken);

        if (httpResponse.StatusCode is not HttpStatusCode.OK)
            return Hub88ResultFactory.Failure<GetHub88LaunchUrlResponseDto>(Hub88ErrorCode.RS_ERROR_UNKNOWN);

        var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        var responseJsonNode = JsonNode.Parse(responseString);

        var responseUrl = responseJsonNode?["url"]?.GetValue<string>();

        if (responseUrl is null)
        {
            var error = responseJsonNode?["error"]!.GetValue<string>()!;
            return Hub88ResultFactory.Failure<GetHub88LaunchUrlResponseDto>(Enum.Parse<Hub88ErrorCode>(error));
        }

        var resultData = new GetHub88LaunchUrlResponseDto(responseUrl);

        return Hub88ResultFactory.Success(resultData);
    }

    public async Task<IPswResult<GetLaunchUrlResponseDto>> GetPswGameLinkAsync(
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
        var response = await _httpClient.PostAsJsonAsync(
            "psw/game/session",
            new
            {
                casinoId,
                sessionId,
                user,
                currency,
                game,
                locale,
                lobby,
                launchMode
            },
            _pswJsonSerializerOptions,
            cancellationToken);

        if (response.StatusCode is not HttpStatusCode.OK)
            return PswResultFactory.Failure<GetLaunchUrlResponseDto>(PswErrorCode.Unknown);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var jsonNode = JsonNode.Parse(responseString);
        var responseStatusString = jsonNode?["status"]?.GetValue<string>();

        if (responseStatusString is not null)
        {
            var responseStatus = Enum.Parse<PswStatus>(responseStatusString);

            if (responseStatus is not PswStatus.ERROR)
                return PswResultFactory.Failure<GetLaunchUrlResponseDto>(PswErrorCode.Unknown);

            var errorModel = jsonNode.Deserialize<PswBaseGamesApiErrorResponseDto>(_pswJsonSerializerOptions)!;
            return PswResultFactory.Failure<GetLaunchUrlResponseDto>(errorModel.Error);
        }

        var successModel = jsonNode.Deserialize<GetLaunchUrlResponseDto>(_pswJsonSerializerOptions)!;

        return PswResultFactory.Success(successModel);
    }

    public async Task<IPswResult<GetCasinoGamesListResponseDto>> GetCasinoGamesAsync(
        string casinoId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "psw/game/list",
            new {casinoId},
            _pswJsonSerializerOptions,
            cancellationToken);

        var responseResult = await response.GetResponseResult<GetCasinoGamesListResponseDto>(
            _pswJsonSerializerOptions,
            cancellationToken);

        return responseResult;
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
        var response = await _httpClient.PostAsJsonAsync(
            "psw/freebet/award",
            new
            {
                casinoId,
                user,
                awardId,
                currency,
                games,
                validUntil,
                count
            },
            _pswJsonSerializerOptions,
            cancellationToken);

        var responseResult = await response.GetResponseResult<CreateFreebetAwardResponseDto>(
            _pswJsonSerializerOptions,
            cancellationToken);

        return responseResult;
    }
}