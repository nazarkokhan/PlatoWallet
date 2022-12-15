namespace Platipus.Wallet.Api.Application.Requests.External;

using Api.Extensions.SecuritySign;
using Base;
using Domain.Entities;
using Domain.Entities.Enums;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.GamesApi;
using Services.GamesGlobalGamesApi;
using Services.Hub88GamesApi;
using Services.Hub88GamesApi.DTOs;
using Services.Hub88GamesApi.DTOs.Requests;
using Services.SoftswissGamesApi;
using StartupSettings.Options;
using Wallets.Psw.Base.Response;

public record LogInRequest(
    string UserName,
    string Password,
    string CasinoId,
    string Game,
    string? Device) : IBaseWalletRequest, IRequest<IPswResult<LogInRequest.Response>> //TODO try IBaseResult
{
    public class Handler : IRequestHandler<LogInRequest, IPswResult<Response>>
    {
        private readonly WalletDbContext _context;
        private readonly IGamesApiClient _gamesApiClient;
        private readonly IHub88GamesApiClient _hub88GamesApiClient;
        private readonly ISoftswissGamesApiClient _softswissGamesApiClient;
        private readonly IGamesGlobalGamesApiClient _globalGamesApiClient;

        public Handler(
            WalletDbContext context,
            IGamesApiClient gamesApiClient,
            IHub88GamesApiClient hub88GamesApiClient,
            ISoftswissGamesApiClient softswissGamesApiClient,
            IGamesGlobalGamesApiClient globalGamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
            _hub88GamesApiClient = hub88GamesApiClient;
            _softswissGamesApiClient = softswissGamesApiClient;
            _globalGamesApiClient = globalGamesApiClient;
        }

        public async Task<IPswResult<Response>> Handle(LogInRequest request, CancellationToken cancellationToken)
        {
            var casino = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                return PswResultFactory.Failure<Response>(PswErrorCode.InvalidCasinoId);

            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.UserName && u.CasinoId == request.CasinoId)
                .Include(u => u.Casino)
                .Include(u => u.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return PswResultFactory.Failure<Response>(PswErrorCode.InvalidUser);

            if (user.IsDisabled)
                return PswResultFactory.Failure<Response>(PswErrorCode.UserDisabled);

            if (user.Password != request.Password)
                return PswResultFactory.Failure<Response>(PswErrorCode.Unknown);

            var session = new Session
            {
                ExpirationDate = DateTime.UtcNow.AddDays(1),
                User = user,
            };

            _context.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            var game = await _context.Set<Game>()
                .Where(g => g.LaunchName == request.Game)
                .Select(
                    g => new
                    {
                        g.GameServerId,
                        g.LaunchName
                    })
                .FirstAsync(cancellationToken);

            string launchUrl;

            switch (casino.Provider)
            {
                //TODO refactor
                case CasinoProvider.Psw:
                {
                    var getGameLinkResult = await _gamesApiClient.GetLaunchUrlAsync(
                        user.Casino.Id,
                        session.Id,
                        user.UserName,
                        user.Currency.Name,
                        request.Game,
                        cancellationToken: cancellationToken);

                    launchUrl = getGameLinkResult.Data?.LaunchUrl ?? "";
                    break;
                }
                case CasinoProvider.Openbox:
                    launchUrl = GetOpenboxLaunchUrl(
                        session.Id,
                        user.CasinoId,
                        user.Id,
                        user.UserName,
                        request.Game,
                        user.Currency.Name);
                    break;
                case CasinoProvider.Dafabet:
                    launchUrl = GetDafabetLaunchUrlAsync(
                        request.Game,
                        user.UserName,
                        session.Id,
                        user.Currency.Name,
                        request.Device,
                        "en",
                        DatabetHash.Compute(
                            $"launch{request.Game}{user.UserName}{session.Id}{user.Currency.Name}",
                            casino.SignatureKey));
                    break;
                case CasinoProvider.Hub88:
                {
                    var getHub88GameLinkRequestDto = new Hub88GetGameLinkGamesApiRequestDto(
                        user.UserName,
                        session.Id.ToString(),
                        user.CasinoId,
                        request.Device ?? "GPL_DESKTOP",
                        user.CasinoId,
                        new Hub88GameServerMetaDto(10, "decimal"),
                        "https://amazing-casino.com/lobby",
                        "en",
                        "142.245.172.168",
                        556,
                        request.Game,
                        "https://amazing-casion.com/deposit",
                        user.Currency.Name,
                        "EE");

                    var getGameLinkResult = await _hub88GamesApiClient.GetLaunchUrlAsync(
                        getHub88GameLinkRequestDto,
                        cancellationToken);

                    launchUrl = getGameLinkResult.Data?.Url ?? "";
                    break;
                }
                case CasinoProvider.Softswiss:
                {
                    var getGameLinkResult = await _softswissGamesApiClient.GetLaunchUrlAsync(
                        user.CasinoId,
                        user.UserName,
                        session.Id,
                        request.Game,
                        user.Currency.Name,
                        (long)user.Balance, //TODO
                        cancellationToken);

                    launchUrl = getGameLinkResult.Data?.LaunchOptions?.GameUrl ?? "";
                    break;
                }
                case CasinoProvider.SoftBet:
                    launchUrl = GetSoftBetLaunchUrlAsync(
                        request.Game,
                        game.GameServerId,
                        session.Id,
                        user.UserName,
                        user.Currency.Name);
                    break;
                case CasinoProvider.GamesGlobal:
                    var getLaunchUrlResult = await _globalGamesApiClient.GetLaunchUrlAsync(
                        session.Id,
                        game.LaunchName,
                        cancellationToken);

                    launchUrl = getLaunchUrlResult.Data ?? "";
                    break;
                default:
                    launchUrl = "";
                    break;
            }

            var result = new Response(session.Id, user.Balance, launchUrl);

            return PswResultFactory.Success(result);
        }
    }

    private async static Task<string> GetLaunchUrl()
    {
        return ""; //TODO
    }

    private static string GetHub88LaunchUrl(
        Guid token,
        string agencyUid,
        Guid playerUid,
        string playerId,
        string gameId,
        string currency)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            // { "brand", "openbox" },//TODO need?
            { nameof(token), token.ToString() },
            { "agency-uid", agencyUid },
            { "player-uid", playerUid.ToString() },
            { "player-type", "1" },
            { "player-id", playerId },
            { "game-id", gameId },
            { "country", "CN" },
            { "language", "en" },
            { nameof(currency), currency },
            // {"backurl", "zero"},
            // {"backUri", "zero"},
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(
            new Uri("https://test.platipusgaming.com/onlinecasino/"),
            $"hub88/launcher{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetOpenboxLaunchUrl(
        Guid token,
        string agencyUid,
        Guid playerUid,
        string playerId,
        string gameId,
        string currency)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            // { "brand", "openbox" },//TODO need?
            { nameof(token), token.ToString() },
            { "agency-uid", agencyUid },
            { "player-uid", playerUid.ToString() },
            { "player-type", "1" },
            { "player-id", playerId },
            { "game-id", gameId },
            { "country", "CN" },
            { "language", "en" },
            { nameof(currency), currency },
            // {"backurl", "zero"},
            // {"backUri", "zero"},
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(
            new Uri("https://test.platipusgaming.com/onlinecasino/"),
            $"openbox/launcher{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetDafabetLaunchUrlAsync(
        string gameCode,
        string playerId,
        Guid playerToken,
        string currency,
        string? device,
        string? language,
        string hash)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            { "brand", "dafabet" },
            { nameof(gameCode), gameCode },
            { nameof(playerId), playerId },
            { nameof(playerToken), playerToken.ToString() },
            { nameof(currency), currency }
        };

        if (device is not null)
            queryParameters.Add(nameof(device), device);

        if (language is not null)
            queryParameters.Add(nameof(language), language);

        queryParameters.Add(nameof(hash), hash);

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(new Uri("https://test.platipusgaming.com/"), $"dafabet/launch{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetSoftBetLaunchUrlAsync(
            string providerGameId,
            int gameId,
            // string licenseeId,
            // string @operator,
            Guid token,
            string username,
            string currency)
        // string country,
        // string isbSkinId,
        // string isbGameId,
        // string mode,
        // string extra)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            { "providergameid", providerGameId },
            { "licenseeid", "134" },
            { "operator", "" },
            { "playerid", username },
            { "token", token.ToString() },
            { "username", username },
            { "currency", currency },
            { "country", "ukraine" },
            { "ISBskinid", "1" },
            { "ISBgameid", "1" },
            { "mode", "real" },
            { "launchercode", "26" },
            { "language", "en" },
            { "lobbyurl", "" },
            { "extra", "" },
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(new Uri("https://test.platipusgaming.com/"), $"softbet/launch{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    public record Response(Guid SessionId, decimal Balance, string LaunchUrl) : PswBalanceResponse(Balance);

    public class Validator : AbstractValidator<SignUpRequest>
    {
        public Validator(IOptions<SupportedCurrenciesOptions> currenciesOptions)
        {
            var currenciesOptionsValue = currenciesOptions.Value;

            RuleFor(x => currenciesOptionsValue.Fiat.Contains(x.Currency) || currenciesOptionsValue.Crypto.Contains(x.Currency));

            RuleFor(p => p.Password).MinimumLength(6).MaximumLength(8);

            RuleFor(p => p.Balance).ScalePrecision(2, 38);
        }
    }
}