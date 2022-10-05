namespace PlatipusWallet.Api.Application.Services.GamesApiService;

using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using DTOs;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;
using Results.External.Enums;

public class GamesApiClient : IGamesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public GamesApiClient(HttpClient httpClient, IOptions<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Value.SerializerOptions;
    }

    public async Task<IResult<GetLaunchUrlResponseDto>> GetGameLinkAsync(
        string casinoId,
        Guid sessionId,
        string user,
        string currency,
        string game = "extragems",
        string locale = "en",
        string lobby = "",
        string launchMode = "url",
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

        var responseResult = await response.GetResponseResult<GetLaunchUrlResponseDto>(
            _jsonSerializerOptions,
            cancellationToken);

        return responseResult;
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

public static class HttpClientExtensions
{
    public static async Task<IResult<T>> GetResponseResult<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken)
    {
        try
        {
            if (response.StatusCode is not HttpStatusCode.OK)
                return ResultFactory.Failure<T>(ErrorCode.Unknown);

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            var jsonNode = JsonNode.Parse(responseString);
            var responseStatusString = jsonNode?["status"]?.GetValue<string>();

            if (responseStatusString is null)
                return ResultFactory.Failure<T>(ErrorCode.Unknown);

            var responseStatus = Enum.Parse<Status>(responseStatusString);

            if (responseStatus is Status.ERROR)
            {
                var errorModel = jsonNode.Deserialize<BaseExternalResponse>(jsonSerializerOptions)!;
                return ResultFactory.Failure<T>(errorModel.Error, errorModel.Description);
            }

            var successModel = jsonNode.Deserialize<T>(jsonSerializerOptions)!;

            return ResultFactory.Success(successModel);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<T>(ErrorCode.Unknown, e);
        }
    }
}