namespace PlatipusWallet.Api.Application.Services.DatabetGamesApi;

using System.Text.Json;
using GamesApi;
using GamesApi.DTOs.Responses;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Results.Common.Result.WithData;

public class DatabetGamesApiClient : IDatabetGamesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public DatabetGamesApiClient(HttpClient httpClient, IOptions<JsonOptions> jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.Value.SerializerOptions;
    }

    public async Task<IResult<GetDatabetLaunchUrlResponseDto>> DatabetLaunchGameAsync(
        string gameCode,
        string playerId,
        Guid playerToken,
        string currency,
        string? language,
        string hash,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            {"brand", "dafabet"},
            {nameof(gameCode), gameCode},
            {nameof(playerId), playerId},
            {nameof(playerToken), playerToken.ToString()},
            {nameof(currency), currency},
        };

        if (language is not null)
            queryParameters.Add(nameof(language), language);

        queryParameters.Add(nameof(hash), hash);

        var queryString = QueryString.Create(queryParameters);

        var response = await _httpClient.PostAsJsonAsync(
            $"launch{queryString.ToUriComponent()}",
            new { },
            _jsonSerializerOptions,
            cancellationToken);

        var responseResult = await response.GetResponseResult<GetDatabetLaunchUrlResponseDto>(
            _jsonSerializerOptions,
            cancellationToken);

        return responseResult;
    }
}