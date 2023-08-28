namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.Hub88GamesApi;

using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using DTOs.Requests;
using DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Platipus.Wallet.Api.Application.Results.Hub88;
using Platipus.Wallet.Api.Application.Results.Hub88.WithData;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Extensions.SecuritySign;
using Platipus.Wallet.Domain.Entities.Enums;

public class Hub88GamesApiClient : IHub88GamesApiClient
{
    private const string Hub88 = "hub88/";
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _hub88JsonSerializerOptions;

    public Hub88GamesApiClient(HttpClient httpClient, IOptionsMonitor<JsonOptions> jsonOptions)
    {
        _httpClient = httpClient;
        _hub88JsonSerializerOptions = jsonOptions.Get(nameof(WalletProvider.Hub88)).JsonSerializerOptions;
    }

    //Game
    public async Task<IHub88Result<Hub88GetLaunchUrlGamesApiResponseDto>> GetLaunchUrlAsync(
        Uri baseUrl,
        Hub88GetGameLinkGamesApiRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88GetGameLinkGamesApiRequestDto, Hub88GetLaunchUrlGamesApiResponseDto>(
            new Uri(baseUrl, Hub88 + "game/url").AbsoluteUri,
            request,
            cancellationToken);

        return response;
    }

    public async Task<IHub88Result<List<Hub88GetGameDto>>> GetGamesListAsync(
        Hub88GetGamesListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88GetGamesListRequestDto, List<Hub88GetGameDto>>(
            Hub88 + "game/list",
            request,
            cancellationToken);

        return response;
    }

    public async Task<IHub88Result<Hub88GetRoundGamesApiResponseDto>> GetRoundAsync(
        Hub88GetRoundGamesApiRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88GetRoundGamesApiRequestDto, Hub88GetRoundGamesApiResponseDto>(
            Hub88 + "game/round",
            request,
            cancellationToken);

        return response;
    }

    //Freebet
    public async Task<IHub88Result<List<Hub88PrepaidGamesApiResponseDto>>> GetPrepaidsListAsync(
        Hub88PrepaidsListGamesApiRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88PrepaidsListGamesApiRequestDto, List<Hub88PrepaidGamesApiResponseDto>>(
            Hub88 + "freebet/prepaids/list",
            request,
            cancellationToken);

        return response;
    }

    public async Task<IHub88Result<Hub88GameApiCreateRewardResponseDto>> CreateRewardAsync(
        Hub88GameApiCreateRewardRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88GameApiCreateRewardRequestDto, Hub88GameApiCreateRewardResponseDto>(
            "freebet/rewards/create",
            request,
            cancellationToken);

        return response;
    }

    public async Task<IHub88Result<Hub88GameApiCancelRewardResponseDto>> CancelRewardAsync(
        Hub88GameApiCancelRewardRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await PostSignedRequestAsync<Hub88GameApiCancelRewardRequestDto, Hub88GameApiCancelRewardResponseDto>(
            "freebet/rewards/create",
            request,
            cancellationToken);

        return response;
    }

    private async Task<IHub88Result<TResponse>> PostSignedRequestAsync<TRequest, TResponse>(
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var jsonContent = await CreateSignedContentAsync(request, cancellationToken);

            var httpResponse = await _httpClient.PostAsync(requestUri, jsonContent, cancellationToken);

            var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(responseString))
                return Hub88ResultFactory.Failure<TResponse>(Hub88ErrorCode.RS_ERROR_UNKNOWN);

            var responseJsonNode = JsonNode.Parse(responseString);

            var error = responseJsonNode?["error"]?.GetValue<string>();

            if (error is not null)
                return Hub88ResultFactory.Failure<TResponse>(Enum.Parse<Hub88ErrorCode>(error));

            if (httpResponse.StatusCode is not HttpStatusCode.OK)
                return Hub88ResultFactory.Failure<TResponse>(Hub88ErrorCode.RS_ERROR_UNKNOWN);

            var response = responseJsonNode.Deserialize<TResponse>(_hub88JsonSerializerOptions);

            if (response is null)
                return Hub88ResultFactory.Failure<TResponse>(Hub88ErrorCode.RS_ERROR_UNKNOWN);

            return Hub88ResultFactory.Success(response);
        }
        catch (Exception e)
        {
            return Hub88ResultFactory.Failure<TResponse>(Hub88ErrorCode.RS_ERROR_UNKNOWN, e);
        }
    }

    private async Task<JsonContent> CreateSignedContentAsync<T>(T request, CancellationToken cancellationToken)
    {
        var jsonContent = JsonContent.Create(request, options: _hub88JsonSerializerOptions);
        var requestBytes = await jsonContent.ReadAsByteArrayAsync(cancellationToken);

        // var jsonText = Encoding.UTF8.GetString(requestBytes);
        // var operatorId = JsonNode.Parse(jsonText)?["operator_id"]?.GetValue<string?>();

        var privateGameServiceSecuritySign = @"-----BEGIN RSA PRIVATE KEY-----
MIICWwIBAAKBgHi57tRMYFBfHa8ZN5NTTSsK/iOKUBmOjhzZKrrZiLjraL/U9edz
ftNi5KaSoXOXLpiEOvaTD+fuuXGvDbME4+XBlfavFX8zza9FDLmERh9uhe+OLgwP
u4AebHvwt8uMY2Eg5+EOc4m0uvlEDI09U54WaxgNw9k4n3mnHboXXVHxAgMBAAEC
gYBtJg2bu4HIqHY5/N6WQFYgeEvU7hQFRzGNO3q6fDp0lcGaznuUyoL7swlu4FtA
GotyMPruO3/B/b+D3PTRybYQlmkLESdrffIwChTckIS29UkEtBAn5QE2ifOaa4Ic
KRACGi0gjOToulTS4HB2/0EWd5ilRjkqhWYvbFm2bHV6kQJBAOWtWZcHtG5BHveu
HWYBh26izHeMSztyTUKRTiC97Fl3/dYmSyEON4fDV9gNervSEvfOJgIpISmxwXeh
wL8sXlsCQQCGkAB5UvykV8aIR8k39f83jZT9e49YdGxXvg6WY8cylyQiO6Mw9mom
NAsqaeDSLbe9MpZroWvVRbvWnV5MGBqjAkEAmN48Vw3FxeyKFAhLgO1bmwO4W4mB
OVvmmHvmKFzAxvvac4KhVqsDwtT9zsuJ+SDlhxIqsh11+S5auqlqhNOfKQJAJiz6
hXEezf09DPLYynCXDIq1Z0jDvUOibS41c0Mxg0/P54pl3QE70kTXmhvZtadUxm9w
r25namVTSirxUsNP4wJAMCOSoOD1vOIVlRQ+qyZm9lEZOGUlSdcx4QLzepNYbEK7
8Dj8gIpH/b8QjPE2emdoYw1VDtUkWxj6dWu1wNLkBA==
-----END RSA PRIVATE KEY-----"; //TODO get from db in Params[]

        var xRequestSign = Hub88SecuritySign.Compute(requestBytes, privateGameServiceSecuritySign);

        jsonContent.Headers.Add(Hub88Headers.XHub88Signature, xRequestSign);

        return jsonContent;
    }
}