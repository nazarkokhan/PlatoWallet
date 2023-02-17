namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi;

using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.Entities.Enums;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Results.Reevo;
using Results.Reevo.WithData;

public class ReevoGameApiClient : IReevoGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _hub88JsonSerializerOptions;

    public ReevoGameApiClient(HttpClient httpClient, IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _hub88JsonSerializerOptions = jsonOptions.Get(nameof(CasinoProvider.Reevo)).JsonSerializerOptions;
    }

    public async Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoGetGameGameApiResponse>>> GetGameAsync(
        ReevoGetGameGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoGetGameGameApiRequest, ReevoGetGameGameApiResponse>(
            request,
            cancellationToken);
    }

    public async Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoAddFreeRoundsGameApiResponse>>> AddFreeRoundAsync(
        ReevoAddFreeRoundsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoAddFreeRoundsGameApiRequest, ReevoAddFreeRoundsGameApiResponse>(
            request,
            cancellationToken);
    }

    public async Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoGetGameHistoryGameApiResponse>>> GetGameHistory(
        ReevoGetGameHistoryGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoGetGameHistoryGameApiRequest, ReevoGetGameHistoryGameApiResponse>(
            request,
            cancellationToken);
    }

    public async Task<IReevoResult<ReevoCommonBoxGameApiResponse<ReevoGetGameListGameApiResponse>>> CreateRewardAsync(
        ReevoGetGameListGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoGetGameListGameApiRequest, ReevoGetGameListGameApiResponse>(
            request,
            cancellationToken);
    }

    private async Task<IReevoResult<ReevoCommonBoxGameApiResponse<TResponse>>> PostSignedRequestAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var jsonContent = JsonContent.Create(request, options: _hub88JsonSerializerOptions);

            var httpResponse = await _httpClient.PostAsync((string?)null, jsonContent, cancellationToken);

            var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(responseString))
                return ReevoResultFactory.Failure<ReevoCommonBoxGameApiResponse<TResponse>>(ReevoErrorCode.GeneralError);

            var responseJsonNode = JsonNode.Parse(responseString);

            var error = responseJsonNode?["error"]?.GetValue<int?>();
            if (error is not 0)
            {
                var errorResponse = responseJsonNode.Deserialize<ReevoErrorGameApiResponse>(_hub88JsonSerializerOptions);
                if (errorResponse is null)
                    return ReevoResultFactory.Failure<ReevoCommonBoxGameApiResponse<TResponse>>(ReevoErrorCode.GeneralError);
                var errorBox = new ReevoCommonBoxGameApiResponse<TResponse>(errorResponse, default!);

                return ReevoResultFactory.Success(errorBox);
            }

            var successResponse = responseJsonNode.Deserialize<TResponse>(_hub88JsonSerializerOptions);
            if (successResponse is null)
                return ReevoResultFactory.Failure<ReevoCommonBoxGameApiResponse<TResponse>>(ReevoErrorCode.GeneralError);
            var successBox = new ReevoCommonBoxGameApiResponse<TResponse>(null, successResponse);

            return ReevoResultFactory.Success(successBox);
        }
        catch (Exception e)
        {
            return ReevoResultFactory.Failure<ReevoCommonBoxGameApiResponse<TResponse>>(ReevoErrorCode.GeneralError, e);
        }
    }
}