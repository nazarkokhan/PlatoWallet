namespace Platipus.Wallet.Api.Application.Services.GamesApi;

using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using DTOs.Base;
using DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Results.Common;
using Results.Common.Result.Factories;
using Results.External.Enums;

public class GamesApiClient : IGamesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public GamesApiClient(HttpClient httpClient, IOptions<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Value.JsonSerializerOptions;
    }

    public async Task<IResult<GetLaunchUrlResponseDto>> GetGameLinkAsync(
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
            "game/session",
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
            _jsonSerializerOptions,
            cancellationToken);

        if (response.StatusCode is not HttpStatusCode.OK)
            return ResultFactory.Failure<GetLaunchUrlResponseDto>(ErrorCode.Unknown);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var jsonNode = JsonNode.Parse(responseString);
        var responseStatusString = jsonNode?["status"]?.GetValue<string>();

        if (responseStatusString is not null)
        {
            var responseStatus = Enum.Parse<Status>(responseStatusString);

            if (responseStatus is not Status.ERROR)
                return ResultFactory.Failure<GetLaunchUrlResponseDto>(ErrorCode.Unknown);
            
            var errorModel = jsonNode.Deserialize<BaseGamesApiErrorResponseDto>(_jsonSerializerOptions)!;
            return ResultFactory.Failure<GetLaunchUrlResponseDto>(errorModel.Error);
        }

        var successModel = jsonNode.Deserialize<GetLaunchUrlResponseDto>(_jsonSerializerOptions)!;

        return ResultFactory.Success(successModel);
    }

    public async Task<IResult<GetCasinoGamesListResponseDto>> GetCasinoGamesAsync(
        string casinoId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "game/list",
            new
            {
                casinoId
            },
            _jsonSerializerOptions,
            cancellationToken);

        var responseResult = await response.GetResponseResult<GetCasinoGamesListResponseDto>(
            _jsonSerializerOptions,
            cancellationToken);

        return responseResult;
    }

    public async Task<IResult<CreateFreebetAwardResponseDto>> CreateFreebetAwardAsync(
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
            "freebet/award",
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
            _jsonSerializerOptions,
            cancellationToken);

        var responseResult = await response.GetResponseResult<CreateFreebetAwardResponseDto>(
            _jsonSerializerOptions,
            cancellationToken);

        return responseResult;
    }
}