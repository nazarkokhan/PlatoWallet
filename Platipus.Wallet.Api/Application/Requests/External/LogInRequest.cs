namespace Platipus.Wallet.Api.Application.Requests.External;

using Api.Extensions.SecuritySign;
using Base;
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
using Services.SoftswissGamesApi;
using StartupSettings.Options;
using Wallets.Psw.Base.Response;

public record LogInRequest(
    string UserName,
    string Password,
    string CasinoId,
    string Game,
    string? Device,
    string? Lobby = "XXX",
    string? UisLaunchType = "Play now") : IBaseWalletRequest, IRequest<IPswResult<LogInRequest.Response>> //TODO try IBaseResult
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
                User = user,
                IsTemporaryToken = casino.Provider is CasinoProvider.Everymatrix //TODO add support for older ones
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
                .FirstOrDefaultAsync(cancellationToken);

            if (game is null)
                return PswResultFactory.Failure<Response>(PswErrorCode.InvalidGame);

            string launchUrl;

            switch (casino.Provider)
            {
                case CasinoProvider.Psw or CasinoProvider.Betflag:
                {
                    var getGameLinkResult = await _gamesApiClient.GetLaunchUrlAsync(
                        user.Casino.Provider!.Value,
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
                        DatabetSecurityHash.Compute(
                            "launch",
                            $"{request.Game}{user.UserName}{session.Id}{user.Currency.Name}",
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
                case CasinoProvider.Sw:
                    launchUrl = GetSwLaunchUrl(
                        session.Id,
                        $"{casino.Id}-{user.Currency.Name}",
                        user.UserName,
                        request.Game);
                    break;
                case CasinoProvider.SoftBet:
                    launchUrl = GetSoftBetLaunchUrlAsync(
                        game.GameServerId,
                        casino.SignatureKey,
                        user.UserName,
                        user.Currency.Name,
                        casino.SwProviderId!.Value);
                    break;
                case CasinoProvider.GamesGlobal:
                    var getLaunchUrlResult = await _globalGamesApiClient.GetLaunchUrlAsync(
                        session.Id,
                        game.LaunchName,
                        cancellationToken);
                    launchUrl = getLaunchUrlResult.Data ?? "";
                    break;
                case CasinoProvider.Uis:
                    launchUrl = GetUisLaunchUrl(
                        session.Id,
                        casino.SwProviderId!.Value,
                        request.UisLaunchType!);
                    break;
                case CasinoProvider.Reevo:
                    var reevoLaunchUrlResult = await _globalGamesApiClient.GetLaunchUrlAsync(
                        session.Id,
                        game.LaunchName,
                        cancellationToken);
                    launchUrl = reevoLaunchUrlResult.Data ?? "";
                    break;
                case CasinoProvider.Everymatrix:
                    launchUrl = GetEveryMatrixLaunchUrlAsync(
                        casino.Id,
                        request.Game,
                        "en",
                        false,
                        false,
                        "dev",
                        session.Id,
                        user.Currency.Name);
                    break;

                //TODO refactor
                // case CasinoProvider.PariMatch:
                // {
                //     launchUrl = GetPariMatchLaunchUrl(
                //         "PlatipusGaming",
                //         )
                // }
                default:
                    launchUrl = "";
                    break;
            }

            var url = new Uri(launchUrl);
            var queryParams = QueryHelpers.ParseQuery(url.Query);
            if (!queryParams.TryGetValue("lobby", out var lobby) || string.IsNullOrWhiteSpace(lobby))
            {
                queryParams["lobby"] = request.Lobby;
            }

            launchUrl = url.AbsoluteUri.Replace(url.Query, null) + QueryString.Create(queryParams);
            var result = new Response(session.Id, user.Balance, launchUrl);

            return PswResultFactory.Success(result);
        }
    }

    private static string GetUisLaunchUrl(
        Guid token,
        int operatorId,
        string launchType)
    {
        var queryParameters = new List<KeyValuePair<string, string?>>();

        var tokenKvp = KeyValuePair.Create("token", token.ToString());
        var operatorIdKvp = KeyValuePair.Create("operatorID", operatorId.ToString());
        var demoKvp = KeyValuePair.Create("demo", bool.TrueString);

        switch (launchType)
        {
            case "Play now":
                queryParameters.Add(tokenKvp!);
                queryParameters.Add(operatorIdKvp!);
                break;
            case "Play now + Demo":
                queryParameters.Add(tokenKvp!);
                queryParameters.Add(operatorIdKvp!);
                queryParameters.Add(demoKvp!);
                break;
            case "Demo":
                queryParameters.Add(demoKvp!);
                break;
        }

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(
            new Uri("https://platipusgaming.cloud/qa/integration/"),
            $"vivo/test/index.html{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetSwLaunchUrl(
        Guid token,
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
            { "token", token.ToString() },
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(
            new Uri("https://test.platipusgaming.com/"),
            $"BIGBOSS/connect.do{queryString.ToUriComponent()}");

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
        var queryParameters = new Dictionary<string, string?>
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
        var queryParameters = new Dictionary<string, string?>
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

        var uri = new Uri(new Uri("https://test.platipusgaming.com/"), $"isoftbet/launch{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetEveryMatrixLaunchUrlAsync(
        string brand,
        string gameCode,
        string? language,
        bool freePlay,
        bool mobile,
        string mode,
        Guid token,
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
            { nameof(token), token.ToString() },
            { nameof(currency), currency }
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(new Uri("https://test.platipusgaming.com/"), $"everymatrix/launch{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetPariMatchLaunchUrl(
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

        var uri = new Uri(new Uri("https://test.platipusgaming.com/"), $"parimatch/launch{queryString.ToUriComponent()}");

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