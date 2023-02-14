// namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi;
//
// using DTOs.Requests;
// using DTOs.Responses;
// using Results.Hub88.WithData;
//
// public record Root
// {
//     [JsonPropertyName("api_login")]
//     public string ApiLogin { get; set; }
//
//     [JsonPropertyName("api_password")]
//     public string ApiPassword { get; set; }
//
//     [JsonPropertyName("method")]
//     public string Method { get; set; }
//
//     [JsonPropertyName("user_id")]
//     public string UserId { get; set; }
//
//     [JsonPropertyName("user_nickname")]
//     public string UserNickname { get; set; }
//
//     [JsonPropertyName("user_password")]
//     public string UserPassword { get; set; }
//
//     [JsonPropertyName("lang")]
//     public string Lang { get; set; }
//
//     [JsonPropertyName("gameid")]
//     public string GameId { get; set; }
//
//     [JsonPropertyName("homeurl")]
//     public string HomeUrl { get; set; }
//
//     [JsonPropertyName("play_for_fun")]
//     public string PlayForFun { get; set; }
//
//     [JsonPropertyName("currency")]
//     public string Currency { get; set; }
//
//     [JsonPropertyName("operator")]
//     public string Operator { get; set; }
//
// }
// public interface IHub88GamesApiClient2
// {
//     //Game
//     Task<IHub88Result<Hub88GetLaunchUrlGamesApiResponseDto>> GetGameAsync(
//         Hub88GetGameLinkGamesApiRequestDto request,
//         CancellationToken cancellationToken = default);
//
//     Task<IHub88Result<List<Hub88GetGameDto>>> GetGamesListAsync(
//         Hub88GetGamesListRequestDto request,
//         CancellationToken cancellationToken = default);
//
//     Task<IHub88Result<Hub88GetRoundGamesApiResponseDto>> GetRoundAsync(
//         Hub88GetRoundGamesApiRequestDto request,
//         CancellationToken cancellationToken = default);
//
//     //Freebet
//     Task<IHub88Result<List<Hub88PrepaidGamesApiResponseDto>>> GetPrepaidsListAsync(
//         Hub88PrepaidsListGamesApiRequestDto request,
//         CancellationToken cancellationToken = default);
//
//     Task<IHub88Result<Hub88GameApiCreateRewardResponseDto>> CreateRewardAsync(
//         Hub88GameApiCreateRewardRequestDto request,
//         CancellationToken cancellationToken = default);
//
//     Task<IHub88Result<Hub88GameApiCancelRewardResponseDto>> CancelRewardAsync(
//         Hub88GameApiCancelRewardRequestDto request,
//         CancellationToken cancellationToken = default);
// }
//
// public class Hub88GamesApiClient : IHub88GamesApiClient2
// {
//     private readonly HttpClient _httpClient;
//     private readonly JsonSerializerOptions _hub88JsonSerializerOptions;
//
//     public Hub88GamesApiClient(HttpClient httpClient, IOptionsMonitor<JsonOptions> jsonOptions)
//     {
//         _httpClient = httpClient;
//         _hub88JsonSerializerOptions = jsonOptions.Get(nameof(CasinoProvider.Hub88)).JsonSerializerOptions;
//     }
//
//     //Game
//     public async Task<IHub88Result<Hub88GetLaunchUrlGamesApiResponseDto>> GetLaunchUrlAsync(
//         Hub88GetGameLinkGamesApiRequestDto request,
//         CancellationToken cancellationToken = default)
//     {
//         var response = await PostSignedRequestAsync<Hub88GetGameLinkGamesApiRequestDto, Hub88GetLaunchUrlGamesApiResponseDto>(
//             "game/url",
//             request,
//             cancellationToken);
//
//         return response;
//     }
//
//     public async Task<IHub88Result<List<Hub88GetGameDto>>> GetGamesListAsync(
//         Hub88GetGamesListRequestDto request,
//         CancellationToken cancellationToken = default)
//     {
//         var response = await PostSignedRequestAsync<Hub88GetGamesListRequestDto, List<Hub88GetGameDto>>(
//             "game/list",
//             request,
//             cancellationToken);
//
//         return response;
//     }
//
//     public async Task<IHub88Result<Hub88GetRoundGamesApiResponseDto>> GetRoundAsync(
//         Hub88GetRoundGamesApiRequestDto request,
//         CancellationToken cancellationToken = default)
//     {
//         var response = await PostSignedRequestAsync<Hub88GetRoundGamesApiRequestDto, Hub88GetRoundGamesApiResponseDto>(
//             "game/round",
//             request,
//             cancellationToken);
//
//         return response;
//     }
//
//     //Freebet
//     public async Task<IHub88Result<List<Hub88PrepaidGamesApiResponseDto>>> GetPrepaidsListAsync(
//         Hub88PrepaidsListGamesApiRequestDto request,
//         CancellationToken cancellationToken = default)
//     {
//         var response = await PostSignedRequestAsync<Hub88PrepaidsListGamesApiRequestDto, List<Hub88PrepaidGamesApiResponseDto>>(
//             "freebet/prepaids/list",
//             request,
//             cancellationToken);
//
//         return response;
//     }
//
//     public async Task<IHub88Result<Hub88GameApiCreateRewardResponseDto>> CreateRewardAsync(
//         Hub88GameApiCreateRewardRequestDto request,
//         CancellationToken cancellationToken = default)
//     {
//         var response = await PostSignedRequestAsync<Hub88GameApiCreateRewardRequestDto, Hub88GameApiCreateRewardResponseDto>(
//             "freebet/rewards/create",
//             request,
//             cancellationToken);
//
//         return response;
//     }
//
//     public async Task<IHub88Result<Hub88GameApiCancelRewardResponseDto>> CancelRewardAsync(
//         Hub88GameApiCancelRewardRequestDto request,
//         CancellationToken cancellationToken = default)
//     {
//         var response = await PostSignedRequestAsync<Hub88GameApiCancelRewardRequestDto, Hub88GameApiCancelRewardResponseDto>(
//             "freebet/rewards/create",
//             request,
//             cancellationToken);
//
//         return response;
//     }
//
//     private async Task<IHub88Result<TResponse>> PostSignedRequestAsync<TRequest, TResponse>(
//         string requestUri,
//         TRequest request,
//         CancellationToken cancellationToken)
//     {
//         try
//         {
//             var jsonContent = await CreateSignedContentAsync(request, cancellationToken);
//
//             var httpResponse = await _httpClient.PostAsync(requestUri, jsonContent, cancellationToken);
//
//             var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
//
//             if (string.IsNullOrWhiteSpace(responseString))
//                 return Hub88ResultFactory.Failure<TResponse>(Hub88ErrorCode.RS_ERROR_UNKNOWN);
//
//             var responseJsonNode = JsonNode.Parse(responseString);
//
//             var error = responseJsonNode?["error"]?.GetValue<string>();
//
//             if (error is not null)
//                 return Hub88ResultFactory.Failure<TResponse>(Enum.Parse<Hub88ErrorCode>(error));
//
//             if (httpResponse.StatusCode is not HttpStatusCode.OK)
//                 return Hub88ResultFactory.Failure<TResponse>(Hub88ErrorCode.RS_ERROR_UNKNOWN);
//
//             var response = responseJsonNode.Deserialize<TResponse>(_hub88JsonSerializerOptions);
//
//             if (response is null)
//                 return Hub88ResultFactory.Failure<TResponse>(Hub88ErrorCode.RS_ERROR_UNKNOWN);
//
//             return Hub88ResultFactory.Success(response);
//         }
//         catch (Exception e)
//         {
//             return Hub88ResultFactory.Failure<TResponse>(Hub88ErrorCode.RS_ERROR_UNKNOWN, e);
//         }
//     }
//
//     private async Task<JsonContent> CreateSignedContentAsync<T>(T request, CancellationToken cancellationToken)
//     {
//         var jsonContent = JsonContent.Create(request, options: _hub88JsonSerializerOptions);
//         var requestBytes = await jsonContent.ReadAsByteArrayAsync(cancellationToken);
//
//         var xRequestSign = Hub88SecuritySign.Compute(requestBytes, Hub88SecuritySign.PrivateKeyGameServer);
//
//         jsonContent.Headers.Add(Hub88Headers.XHub88Signature, xRequestSign);
//
//         return jsonContent;
//     }
// }

