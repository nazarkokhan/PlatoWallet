namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi;

using System.Text.Json;
using System.Text.Json.Nodes;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Domain.Entities.Enums;

public class ReevoGameApiClient : IReevoGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _hub88JsonSerializerOptions;

    public ReevoGameApiClient(HttpClient httpClient, IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _hub88JsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Reevo)).JsonSerializerOptions;
    }

    public async Task<IResult<ReevoCommonBoxGameApiResponse<ReevoGetGameGameApiResponse>>> GetGameAsync(
        Uri baseUrl,
        ReevoGetGameGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoGetGameGameApiRequest, ReevoGetGameGameApiResponse>(
            baseUrl,
            request,
            cancellationToken);
    }

    public async Task<IResult<ReevoCommonBoxGameApiResponse<ReevoAddFreeRoundsGameApiResponse>>> AddFreeRoundsAsync(
        Uri baseUrl,
        ReevoAddFreeRoundsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoAddFreeRoundsGameApiRequest, ReevoAddFreeRoundsGameApiResponse>(
            baseUrl,
            request,
            cancellationToken);
    }

    public async Task<IResult<ReevoCommonBoxGameApiResponse<ReevoErrorGameApiResponse>>> RemoveFreeRoundsAsync(
        Uri baseUrl,
        ReevoRemoveFreeRoundsGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoRemoveFreeRoundsGameApiRequest, ReevoErrorGameApiResponse>(
            baseUrl,
            request,
            cancellationToken);
    }

    public async Task<IResult<ReevoCommonBoxGameApiResponse<ReevoGetGameHistoryGameApiResponse>>> GetGameHistoryAsync(
        Uri baseUrl,
        ReevoGetGameHistoryGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoGetGameHistoryGameApiRequest, ReevoGetGameHistoryGameApiResponse>(
            baseUrl,
            request,
            cancellationToken);
    }

    public async Task<IResult<ReevoCommonBoxGameApiResponse<ReevoGetGameListGameApiResponse>>> GetGameListAsync(
        Uri baseUrl,
        ReevoGetGameListGameApiRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostSignedRequestAsync<ReevoGetGameListGameApiRequest, ReevoGetGameListGameApiResponse>(
            baseUrl,
            request,
            cancellationToken);
    }

    private async Task<IResult<ReevoCommonBoxGameApiResponse<TResponse>>> PostSignedRequestAsync<TRequest, TResponse>(
        Uri baseUrl,
        TRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            baseUrl = new Uri(baseUrl, "reevo");

            var jsonContent = JsonContent.Create(request, options: _hub88JsonSerializerOptions);

            var httpResponse = await _httpClient.PostAsync(baseUrl, jsonContent, cancellationToken);

            var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(responseString))
                return ResultFactory.Failure<ReevoCommonBoxGameApiResponse<TResponse>>(ErrorCode.EmptyExternalResponse);

            var responseJsonNode = JsonNode.Parse(responseString);

            var error = responseJsonNode?["error"]?.GetValue<int?>();
            if (error is not 0)
            {
                var errorResponse = responseJsonNode.Deserialize<ReevoErrorGameApiResponse>(_hub88JsonSerializerOptions);
                if (errorResponse is null)
                    return ResultFactory.Failure<ReevoCommonBoxGameApiResponse<TResponse>>(ErrorCode.InvalidExternalResponse);
                var errorBox = new ReevoCommonBoxGameApiResponse<TResponse>(errorResponse, default!);

                return ResultFactory.Success(errorBox);
            }

            var successResponse = responseJsonNode.Deserialize<TResponse>(_hub88JsonSerializerOptions);
            if (successResponse is null)
                return ResultFactory.Failure<ReevoCommonBoxGameApiResponse<TResponse>>(ErrorCode.InvalidExternalResponse);
            var successBox = new ReevoCommonBoxGameApiResponse<TResponse>(null, successResponse);

            return ResultFactory.Success(successBox);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<ReevoCommonBoxGameApiResponse<TResponse>>(ErrorCode.Unknown, e);
        }
    }
}