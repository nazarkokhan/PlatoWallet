namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;

// public interface ISwGamesApiClient
// {
//     Task<ISoftswissResult<SoftswissGetGameLinkGameApiResponse>> GetLaunchUrlAsync(
//         string casinoId,
//         string user,
//         Guid sessionId,
//         string game,
//         string currency,
//         long balance,
//         CancellationToken cancellationToken = default);
// }
//
// public class SwGamesApiClient : ISwGamesApiClient
// {
//     private readonly ILogger<SoftswissGamesApiClient> _logger;
//     private readonly HttpClient _httpClient;
//     private readonly JsonSerializerOptions _softswissJsonSerializerOptions;
//     private readonly IServiceScopeFactory _scopeFactory;
//     private readonly SoftswissCurrenciesOptions _currencyMultipliers;
//
//     public SoftswissGamesApiClient(
//         HttpClient httpClient,
//         IOptionsMonitor<JsonOptions> jsonOptions,
//         IServiceScopeFactory scopeFactory,
//         ILogger<SoftswissGamesApiClient> logger,
//         IOptions<SoftswissCurrenciesOptions> currencyMultipliers)
//     {
//         _httpClient = httpClient;
//         _scopeFactory = scopeFactory;
//         _logger = logger;
//         _currencyMultipliers = currencyMultipliers.Value;
//         _softswissJsonSerializerOptions = jsonOptions.Get(nameof(CasinoProvider.Sw)).JsonSerializerOptions;
//     }
// }