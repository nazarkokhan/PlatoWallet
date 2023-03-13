namespace Platipus.Wallet.Api.Application.Requests.External;

using System.ComponentModel;
using Api.Extensions.SecuritySign;
using Domain.Entities;
using Domain.Entities.Enums;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.GamesApi;
using Services.GamesGlobalGamesApi;
using Services.Hub88GamesApi;
using Services.Hub88GamesApi.DTOs;
using Services.Hub88GamesApi.DTOs.Requests;
using Services.ReevoGamesApi;
using Services.ReevoGamesApi.DTO;
using Services.SoftswissGamesApi;
using StartupSettings.Options;
using Wallets.Psw.Base.Response;

//TODO launchmode for oponbox, uis, psw
public record LogInRequest(
        [property: DefaultValue("openbox")] string UserName,
        [property: DefaultValue("password")] string Password,
        [property: DefaultValue("openbox")] string CasinoId,
        [property: DefaultValue("extragems")] string Game,
        [property: DefaultValue("test")] string? Environment,
        [property: DefaultValue("treba_default_a")] string? Lobby,
        LaunchMode LaunchMode,
        [property: DefaultValue(null)] string? Device = null)
    : IRequest<IResult<LogInRequest.Response>>
{
    public class Handler : IRequestHandler<LogInRequest, IResult<Response>>
    {
        private readonly WalletDbContext _context;
        private readonly IGamesApiClient _gamesApiClient;
        private readonly IHub88GamesApiClient _hub88GamesApiClient;
        private readonly ISoftswissGamesApiClient _softswissGamesApiClient;
        private readonly IGamesGlobalGamesApiClient _globalGamesApiClient;
        private readonly IReevoGameApiClient _reevoGameApiClient;

        public Handler(
            WalletDbContext context,
            IGamesApiClient gamesApiClient,
            IHub88GamesApiClient hub88GamesApiClient,
            ISoftswissGamesApiClient softswissGamesApiClient,
            IGamesGlobalGamesApiClient globalGamesApiClient,
            IReevoGameApiClient reevoGameApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
            _hub88GamesApiClient = hub88GamesApiClient;
            _softswissGamesApiClient = softswissGamesApiClient;
            _globalGamesApiClient = globalGamesApiClient;
            _reevoGameApiClient = reevoGameApiClient;
        }

        public async Task<IResult<Response>> Handle(LogInRequest request, CancellationToken cancellationToken)
        {
            var casino = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                return ResultFactory.Failure<Response>(ErrorCode.CasinoNotFound);

            var user = await _context.Set<User>()
                .Where(u => u.Username == request.UserName && u.CasinoId == request.CasinoId)
                .Include(u => u.Casino)
                .Include(u => u.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<Response>(ErrorCode.UserNotFound);

            if (user.IsDisabled)
                return ResultFactory.Failure<Response>(ErrorCode.UserIsDisabled);

            if (user.Password != request.Password && request.Password != "password")
                return ResultFactory.Failure<Response>(ErrorCode.InvalidPassword);

            var session = new Session
            {
                User = user,
                IsTemporaryToken = true
            };

            if (casino.Provider is not CasinoProvider.Reevo)
            {
                _context.Add(session);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var game = await _context.Set<Game>()
                .Where(g => g.LaunchName == request.Game)
                .Select(
                    g => new
                    {
                        GameServerId = g.GameServiceId,
                        g.LaunchName
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (game is null)
                return ResultFactory.Failure<Response>(ErrorCode.GameNotFound);

            var environmentName = request.Environment ?? "test";
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == environmentName)
                .Select(
                    e => new
                    {
                        e.BaseUrl,
                        e.UisBaseUrl,
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure<Response>(ErrorCode.EnvironmentDoesNotExists);

            var baseUrl = casino.Provider is CasinoProvider.Uis ? environment.UisBaseUrl : environment.BaseUrl;

            string launchUrl;
            switch (casino.Provider)
            {
                case CasinoProvider.Psw or CasinoProvider.Betflag:
                {
                    var getGameLinkResult = await _gamesApiClient.GetLaunchUrlAsync(
                        baseUrl,
                        user.Casino.Provider,
                        user.Casino.Id,
                        session.Id,
                        user.Username,
                        user.Currency.Id,
                        request.Game,
                        request.LaunchMode,
                        cancellationToken: cancellationToken);

                    launchUrl = getGameLinkResult.Data?.LaunchUrl ?? "";
                    break;
                }
                case CasinoProvider.Openbox:
                    launchUrl = GetOpenboxLaunchUrl(
                        baseUrl,
                        session.Id,
                        user.CasinoId,
                        user.Id.ToString(),
                        user.Username,
                        request.Game,
                        user.CurrencyId);
                    break;
                case CasinoProvider.Dafabet:
                    launchUrl = GetDafabetLaunchUrlAsync(
                        baseUrl,
                        request.Game,
                        user.Username,
                        session.Id,
                        user.Currency.Id,
                        request.Device,
                        "en",
                        DatabetSecurityHash.Compute(
                            "launch",
                            $"{request.Game}{user.Username}{session.Id}{user.Currency.Id}",
                            casino.SignatureKey));
                    break;
                case CasinoProvider.Hub88:
                {
                    var getHub88GameLinkRequestDto = new Hub88GetGameLinkGamesApiRequestDto(
                        user.Username,
                        session.Id,
                        user.CasinoId,
                        request.Device ?? "GPL_DESKTOP",
                        user.CasinoId,
                        new Hub88GameServerMetaDto(10, "decimal"),
                        "https://amazing-casino.com/lobby",
                        "en",
                        "142.245.172.168",
                        game.GameServerId,
                        request.Game,
                        "https://amazing-casion.com/deposit",
                        user.Currency.Id,
                        "EE");

                    var getGameLinkResult = await _hub88GamesApiClient.GetLaunchUrlAsync(
                        baseUrl,
                        getHub88GameLinkRequestDto,
                        cancellationToken);

                    launchUrl = getGameLinkResult.Data?.Url ?? "";
                    break;
                }
                case CasinoProvider.Softswiss:
                {
                    var getGameLinkResult = await _softswissGamesApiClient.GetLaunchUrlAsync(
                        baseUrl,
                        user.CasinoId,
                        user.Username,
                        session.Id,
                        game.GameServerId,
                        user.Currency.Id,
                        (long)user.Balance, //TODO //TODO why i left first todo here?
                        cancellationToken);

                    launchUrl = getGameLinkResult.Data?.LaunchOptions?.GameUrl ?? "";
                    break;
                }
                case CasinoProvider.Sw:
                    launchUrl = GetSwLaunchUrl(
                        baseUrl,
                        session.Id,
                        $"{casino.Id}-{user.Currency.Id}",
                        user.Username,
                        request.Game);
                    break;
                case CasinoProvider.SoftBet:
                    launchUrl = GetSoftBetLaunchUrlAsync(
                        baseUrl,
                        game.GameServerId,
                        casino.SignatureKey,
                        user.Username,
                        user.Currency.Id,
                        casino.InternalId);
                    break;
                case CasinoProvider.GamesGlobal:
                    var getLaunchUrlResult = await _globalGamesApiClient.GetLaunchUrlAsync(
                        baseUrl,
                        session.Id,
                        game.LaunchName,
                        cancellationToken);
                    launchUrl = getLaunchUrlResult.Data ?? "";
                    break;
                case CasinoProvider.Uis:
                    launchUrl = GetUisLaunchUrl(
                        environment.UisBaseUrl,
                        session.Id,
                        casino.InternalId,
                        request.LaunchMode);
                    break;
                case CasinoProvider.Reevo:
                    var reevoLaunchUrlResult = await _reevoGameApiClient.GetGameAsync(
                        baseUrl,
                        new ReevoGetGameGameApiRequest(
                            "",
                            "",
                            user.Username,
                            request.UserName,
                            request.Password,
                            "en",
                            game.GameServerId.ToString(),
                            request.Lobby ?? "",
                            "0",
                            user.Currency.Id,
                            casino.Id),
                        cancellationToken);

                    if (reevoLaunchUrlResult.IsFailure || reevoLaunchUrlResult.Data.ErrorMessage is not null)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    var dataSuccess = reevoLaunchUrlResult.Data.Success;

                    session.Id = dataSuccess.GameSessionId;
                    _context.Add(session);
                    await _context.SaveChangesAsync(cancellationToken);

                    launchUrl = dataSuccess.Response;
                    break;
                case CasinoProvider.Everymatrix:
                    launchUrl = GetEveryMatrixLaunchUrlAsync(
                        baseUrl,
                        casino.Id,
                        request.Game,
                        "en",
                        false,
                        false,
                        "dev",
                        session.Id,
                        user.Currency.Id);
                    break;
                default:
                    launchUrl = "";
                    break;
            }

            if (!string.IsNullOrWhiteSpace(launchUrl))
            {
                var url = new Uri(launchUrl);
                var queryParams = QueryHelpers.ParseQuery(url.Query);
                if (!queryParams.TryGetValue("lobby", out var lobby) || string.IsNullOrWhiteSpace(lobby))
                {
                    queryParams["lobby"] = request.Lobby;
                }

                launchUrl = url.AbsoluteUri.Replace(url.Query, null) + QueryString.Create(queryParams);
            }

            var result = new Response(session.Id, user.Balance, launchUrl);

            return ResultFactory.Success(result);
        }
    }

    private static string GetUisLaunchUrl(
        Uri baseUri,
        string token,
        int operatorId,
        LaunchMode launchType)
    {
        var queryParameters = new List<KeyValuePair<string, string?>>();

        var tokenKvp = KeyValuePair.Create("token", token);
        var operatorIdKvp = KeyValuePair.Create("operatorID", operatorId.ToString());
        var demoKvp = KeyValuePair.Create("demo", bool.TrueString);

        switch (launchType)
        {
            //Play now
            case LaunchMode.Real:
                queryParameters.Add(tokenKvp!);
                queryParameters.Add(operatorIdKvp!);
                break;
            //Play now + Demo
            case LaunchMode.Fun:
                queryParameters.Add(tokenKvp!);
                queryParameters.Add(operatorIdKvp!);
                queryParameters.Add(demoKvp!);
                break;
            //Demo
            case LaunchMode.Demo:
                queryParameters.Add(demoKvp!);
                break;
        }

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(baseUri, queryString.ToUriComponent());

        return uri.AbsoluteUri;
    }

    private static string GetSwLaunchUrl(
        Uri baseUrl,
        string token,
        string key,
        string userId,
        string game)
    {
        var queryParameters = new Dictionary<string, string?>
        {
            { "key", key },
            { "userid", userId },
            { "gameconfig", game },
            { "lang", "en" },
            // { "lobby", "" },
            { "token", token },
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(
            baseUrl,
            $"BIGBOSS/connect.do{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetOpenboxLaunchUrl(
        Uri baseUrl,
        string token,
        string agencyUid,
        string playerUid,
        string playerId,
        string gameId,
        string currency)
    {
        var queryParameters = new Dictionary<string, string?>
        {
            // { "brand", "openbox" },//TODO need?
            { nameof(token), token },
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
            baseUrl,
            $"onlinecasino/openbox/launcher{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetDafabetLaunchUrlAsync(
        Uri baseUrl,
        string gameCode,
        string playerId,
        string playerToken,
        string currency,
        string? device,
        string? language,
        string hash)
    {
        var queryParameters = new Dictionary<string, string?>
        {
            { "brand", "dafabet" },
            { nameof(gameCode), gameCode },
            { nameof(playerId), playerId },
            { nameof(playerToken), playerToken },
            { nameof(currency), currency }
        };

        if (device is not null)
            queryParameters.Add(nameof(device), device);

        if (language is not null)
            queryParameters.Add(nameof(language), language);

        queryParameters.Add(nameof(hash), hash);

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(baseUrl, $"dafabet/launch{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetSoftBetLaunchUrlAsync(
        Uri baseUrl,
        int gameId,
        string token,
        string username,
        string currency,
        int licenseeid)
    {
        var queryParameters = new Dictionary<string, string?>
        {
            { "providergameid", gameId.ToString() },
            { "licenseeid", licenseeid.ToString() }, //provider
            { "operator", "" },
            { "playerid", username },
            { "token", token },
            { "username", username },
            { "currency", currency },
            { "country", "ua" },
            { "ISBskinid", "1" },
            { "ISBgameid", "1" },
            { "mode", "real" },
            { "launchercode", "26" },
            { "language", "en" },
            // { "lobbyurl", "" },
            { "extra", "multi" },
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(baseUrl, $"isoftbet/launch{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetEveryMatrixLaunchUrlAsync(
        Uri baseUrl,
        string brand,
        string gameCode,
        string? language,
        bool freePlay,
        bool mobile,
        string mode,
        string token,
        string currency)
    {
        var queryParameters = new Dictionary<string, string?>
        {
            { nameof(brand), brand },
            { nameof(gameCode), gameCode },
            { nameof(language), language },
            { nameof(freePlay), freePlay.ToString().ToLower() },
            { nameof(mobile), mobile.ToString().ToLower() },
            { nameof(mode), mode },
            { nameof(token), token },
            { nameof(currency), currency }
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(baseUrl, $"everymatrix/launch{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetPariMatchLaunchUrl(
        Uri baseUrl,
        string cid,
        string? productId,
        string sessionToken,
        string lang,
        string lobbyUrl,
        string targetChannel,
        string providerId,
        string consumerId)
    {
        var queryParameters = new Dictionary<string, string?>
        {
            { nameof(cid), cid },
            { nameof(productId), productId },
            { nameof(sessionToken), sessionToken },
            { nameof(lang), lang },
            // { nameof(lobbyUrl), lobbyUrl },
            { nameof(targetChannel), targetChannel },
            { nameof(providerId), providerId },
            { nameof(consumerId), consumerId }
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(baseUrl, $"parimatch/launch{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    public record Response(
        string SessionId,
        decimal Balance,
        string LaunchUrl) : PswBalanceResponse(Balance);

    public class Validator : AbstractValidator<SignUpRequest>
    {
        public Validator(IOptions<SupportedCurrenciesOptions> currenciesOptions)
        {
            var currenciesOptionsValue = currenciesOptions.Value;

            RuleFor(x => currenciesOptionsValue.Items.Contains(x.Currency));

            RuleFor(p => p.Balance).ScalePrecision(2, 28);
        }
    }
}