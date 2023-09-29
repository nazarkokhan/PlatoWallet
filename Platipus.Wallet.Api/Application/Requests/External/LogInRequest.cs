namespace Platipus.Wallet.Api.Application.Requests.External;

using System.ComponentModel;
using System.Text;
using Api.Extensions.SecuritySign;
using Domain.Entities;
using Domain.Entities.Enums;
using FluentValidation;
using Helpers;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Results.Base;
using Services;
using Services.AnakatechGameApi;
using Services.AnakatechGameApi.Requests;
using Services.AtlasGameApi;
using Services.AtlasGameApi.Requests;
using Services.EmaraPlayGameApi;
using Services.EmaraPlayGameApi.Requests;
using Services.EvenbetGameApi;
using Services.EvenbetGameApi.Requests;
using Services.Hub88GamesApi;
using Services.Hub88GamesApi.DTOs;
using Services.Hub88GamesApi.DTOs.Requests;
using Services.NemesisGameApi;
using Services.NemesisGameApi.Requests;
using Services.ObsoleteGameApiStyle.ReevoGamesApi;
using Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;
using Services.ObsoleteGameApiStyle.SoftswissGamesApi;
using Services.ParimatchGameApi;
using Services.ParimatchGameApi.Requests;
using Services.PswGameApi;
using Services.SoftBetGameApi;
using Services.SoftBetGameApi.External;
using Services.SynotGameApi;
using Services.SynotGameApi.Requests;
using Services.UranusGamesApi;
using Services.UranusGamesApi.Abstaction;
using Services.UranusGamesApi.Requests;
using Services.VegangsterGameApi;
using Services.VegangsterGameApi.External;
using StartupSettings.Factories;
using StartupSettings.Options;
using Wallets.Anakatech.Enums;
using Wallets.Psw.Base.Response;

public sealed record LogInRequest(
        [property: DefaultValue("openbox")] string UserName,
        [property: DefaultValue("password")] string Password,
        [property: DefaultValue("openbox")] string CasinoId,
        [property: DefaultValue("extragems")] string Game,
        [property: DefaultValue("test")] string Environment,
        [property: DefaultValue("some_lobby_url")] string Lobby,
        LaunchMode LaunchMode,
        [property: DefaultValue(null)] int? PswRealityCheck,
        [property: DefaultValue(null)] string? Device,
        [property: DefaultValue("en")] string Language,
        [property: DefaultValue("https://nashbet.test.k8s-hz.atlas-iac.com/account/payment/deposit")] string? Cashier,
        [property: DefaultValue(null)] string? CustomerId,
        [property: DefaultValue(null)] string? BrandId,
        [property: DefaultValue(null)] string? SecurityToken,
        [property: DefaultValue(null)] string? Nickname,
        [property: DefaultValue(null)] int? Balance,
        [property: DefaultValue(null)] string? Country,
        [property: DefaultValue(null)] string? Jurisdiction,
        [property: DefaultValue(null)] string? OriginUrl,
        [property: DefaultValue(null)] int? RealityCheckInterval,
        [property: DefaultValue(null)] string? DepositUrl)
    : IRequest<IResult<LogInRequest.Response>>
{
    public class Handler : IRequestHandler<LogInRequest, IResult<Response>>
    {
        private readonly WalletDbContext _context;
        private readonly IPswGameApiClient _pswGameApiClient;
        private readonly IHub88GameApiClient _hub88GameApiClient;
        private readonly ISoftswissGamesApiClient _softswissGamesApiClient;
        private readonly IEmaraPlayGameApiClient _emaraPlayGameApiClient;
        private readonly IReevoGameApiClient _reevoGameApiClient;
        private readonly SoftswissCurrenciesOptions _currencyMultipliers;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAtlasGameApiClient _atlasGameApiClient;
        private readonly IUranusGameApiClient _uranusGameApiClient;
        private readonly IEvenbetGameApiClient _evenbetGameApiClient;
        private readonly IAnakatechGameApiClient _anakatechGameApiClient;
        private readonly INemesisGameApiClient _nemesisGameApiClient;
        private readonly IParimatchGameApiClient _parimatchGameApiClient;
        private readonly ISynotGameApiClient _synotGameApiClient;
        private readonly IVegangsterGameApiClient _vegangsterGameApiClient;
        private readonly ISoftBetGameApiClient _softBetGameApiClient;

        public Handler(
            WalletDbContext context,
            IPswGameApiClient pswGameApiClient,
            IHub88GameApiClient hub88GameApiClient,
            ISoftswissGamesApiClient softswissGamesApiClient,
            IReevoGameApiClient reevoGameApiClient,
            IOptions<SoftswissCurrenciesOptions> currencyMultipliers,
            IHttpContextAccessor httpContextAccessor,
            IEmaraPlayGameApiClient emaraPlayGameApiClient,
            IAtlasGameApiClient atlasGameApiClient,
            IUranusGameApiClient uranusGameApiClient,
            IEvenbetGameApiClient evenbetGameApiClient,
            IAnakatechGameApiClient anakatechGameApiClient,
            INemesisGameApiClient nemesisGameApiClient,
            IParimatchGameApiClient parimatchGameApiClient,
            ISynotGameApiClient synotGameApiClient,
            IVegangsterGameApiClient vegangsterGameApiClient,
            ISoftBetGameApiClient softBetGameApiClient)
        {
            _context = context;
            _pswGameApiClient = pswGameApiClient;
            _hub88GameApiClient = hub88GameApiClient;
            _softswissGamesApiClient = softswissGamesApiClient;
            _reevoGameApiClient = reevoGameApiClient;
            _httpContextAccessor = httpContextAccessor;
            _emaraPlayGameApiClient = emaraPlayGameApiClient;
            _atlasGameApiClient = atlasGameApiClient;
            _uranusGameApiClient = uranusGameApiClient;
            _evenbetGameApiClient = evenbetGameApiClient;
            _anakatechGameApiClient = anakatechGameApiClient;
            _nemesisGameApiClient = nemesisGameApiClient;
            _parimatchGameApiClient = parimatchGameApiClient;
            _synotGameApiClient = synotGameApiClient;
            _vegangsterGameApiClient = vegangsterGameApiClient;
            _softBetGameApiClient = softBetGameApiClient;
            _currencyMultipliers = currencyMultipliers.Value;
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

            if (casino.Provider != WalletProvider.Reevo || casino.Provider != WalletProvider.Softswiss)
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

            var environmentName = request.Environment;
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
                return ResultFactory.Failure<Response>(ErrorCode.EnvironmentNotFound);

            var baseUrl = casino.Provider is WalletProvider.Uis ? environment.UisBaseUrl : environment.BaseUrl;

            string? httpRequestMessage = null;
            string? httpResponseMessage = null;
            string launchUrl;
            switch (casino.Provider)
            {
                case WalletProvider.Vegangster:
                {
                    var playerIp = GetPlayerIp();
                    var isDemoLaunchMode = request.LaunchMode is LaunchMode.Demo;

                    IVegangsterCommonGetLaunchUrlApiRequest getLaunchUrlApiRequest = isDemoLaunchMode
                        ? new VegangsterGetDemoLaunchUrlGameApiRequest(
                            request.BrandId!,
                            request.Game,
                            request.Device!,
                            user.CurrencyId,
                            LobbyUrl: request.Lobby,
                            Lang: request.Language,
                            Country: request.Country!,
                            Ip: playerIp)
                        : new VegangsterGetLaunchUrlGameApiRequest(
                            request.BrandId!,
                            user.Id.ToString(),
                            session.Id,
                            request.Game,
                            request.Device!,
                            user.CurrencyId,
                            request.Language,
                            request.Country!,
                            playerIp,
                            request.Lobby,
                            request.DepositUrl!,
                            request.Nickname!);

                    var apiResponse = isDemoLaunchMode
                        ? await _vegangsterGameApiClient.GetDemoLaunchUrlAsync(
                            baseUrl,
                            casino.Id,
                            casino.Params.VegangsterPrivateWalletSecuritySign,
                            getLaunchUrlApiRequest,
                            cancellationToken: cancellationToken)
                        : await _vegangsterGameApiClient.GetLaunchUrlAsync(
                            baseUrl,
                            casino.Id,
                            casino.Params.VegangsterPrivateWalletSecuritySign,
                            getLaunchUrlApiRequest,
                            cancellationToken: cancellationToken);

                    if (apiResponse.IsFailure || apiResponse.Data.IsFailure)
                    {
                        launchUrl = string.Empty;
                        break;
                    }

                    launchUrl = apiResponse.Data.Data.Url.ToString();
                    break;
                }

                case WalletProvider.Synot:
                {
                    var real = request.LaunchMode is LaunchMode.Real;

                    var launchGame = new SynotGetGameLaunchScriptGameApiRequest(
                        request.Game,
                        session.Id,
                        user.CurrencyId,
                        request.Language,
                        real,
                        request.Lobby);

                    var launcherResult = await _synotGameApiClient.GetGameLaunchScriptAsync(
                        casino.Id,
                        baseUrl,
                        launchGame,
                        cancellationToken);

                    if (launcherResult.IsFailure || launcherResult.Data.IsFailure)
                    {
                        launchUrl = string.Empty;
                        break;
                    }

                    launchUrl = ScriptHelper.ExtractUrlFromScript(launcherResult.Data.Data, request.Environment);

                    var lastUsersSession = await _context.Set<Session>()
                       .Include(u => u.User)
                       .Where(s => s.UserId == user.Id)
                       .Select(
                            s => new
                            {
                                s.Id,
                                s.CreatedDate
                            })
                       .OrderByDescending(x => x.CreatedDate)
                       .FirstOrDefaultAsync(cancellationToken);

                    session.Id = lastUsersSession?.Id!;

                    break;
                }

                case WalletProvider.Parimatch:
                {
                    var launcherRequest = new ParimatchLauncherGameApiRequest(
                        casino.Id,
                        game.LaunchName,
                        session.Id,
                        request.Language,
                        request.Lobby,
                        user.Username,
                        casino.InternalId.ToString(),
                        casino.InternalId.ToString());

                    var launcherResult = await _parimatchGameApiClient.LauncherAsync(
                        baseUrl,
                        launcherRequest,
                        cancellationToken);

                    if (launcherResult.IsFailure || launcherResult.Data.IsFailure)
                    {
                        launchUrl = string.Empty;
                        break;
                    }

                    launchUrl = ScriptHelper.ExtractUrlFromScript(launcherResult.Data.Data, request.Environment);
                    break;
                }

                case WalletProvider.Nemesis:
                {
                    var launcherRequest = new NemesisLauncherGameApiRequest(
                        session.Id,
                        game.LaunchName,
                        request.Language,
                        user.Username,
                        user.CurrencyId,
                        casino.Id,
                        request.Device,
                        request.Lobby);

                    var launcherResult = await _nemesisGameApiClient.LauncherAsync(
                        baseUrl,
                        launcherRequest,
                        casino.SignatureKey,
                        cancellationToken);

                    if (launcherResult.IsFailure || launcherResult.Data.IsFailure)
                    {
                        launchUrl = string.Empty;
                        break;
                    }

                    launchUrl = ScriptHelper.ExtractUrlFromScript(launcherResult.Data.Data, request.Environment);
                    break;
                }

                case WalletProvider.Anakatech:
                {
                    var playMode = request.LaunchMode is LaunchMode.Real
                        ? (int)AnakatechPlayMode.RealMoney
                        : (int)AnakatechPlayMode.Anonymous;

                    var apiRequest = new AnakatechLaunchGameApiRequest(
                        request.CustomerId!,
                        request.BrandId!,
                        session.Id,
                        request.SecurityToken!,
                        user.Id.ToString(),
                        request.Game,
                        playMode,
                        request.Nickname!,
                        request.Balance,
                        user.Currency.Id,
                        request.Language,
                        request.Country!,
                        request.Lobby,
                        request.Jurisdiction!,
                        request.OriginUrl!,
                        request.RealityCheckInterval);

                    var apiResponse = await _anakatechGameApiClient.GetLaunchGameUrlAsBytesAsync(
                        baseUrl,
                        apiRequest,
                        cancellationToken: cancellationToken);

                    if (apiResponse.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    var script = apiResponse.Data.Data;

                    var environmentToUse = request.Environment ?? "local";
                    if (environmentToUse is "test")
                    {
                        environmentToUse = "local";
                    }

                    launchUrl = ScriptHelper.ExtractUrlFromScript(script, environmentToUse);

                    break;
                }

                case WalletProvider.Evenbet:
                {
                    var isDemoLaunchMode = request.LaunchMode is LaunchMode.Demo;

                    var apiRequest = new EvenbetGetLaunchGameUrlGameApiRequest(
                        request.Game,
                        isDemoLaunchMode,
                        request.CasinoId,
                        request.Language,
                        request.Device!,
                        user.Currency.Id,
                        session.Id);

                    var apiResponse = await _evenbetGameApiClient.GetGameLaunchUrlAsync(
                        baseUrl,
                        apiRequest,
                        cancellationToken: cancellationToken);

                    if (apiResponse.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    var script = apiResponse.Data.Data;

                    launchUrl = ScriptHelper.ExtractUrlFromScript(script, request.Environment ?? "local");
                    break;
                }

                case WalletProvider.Uranus:
                {
                    var playerIp = GetPlayerIp();
                    var isDemoLaunchMode = request.LaunchMode is LaunchMode.Demo;

                    IUranusCommonGetLaunchUrlApiRequest apiApiRequest = isDemoLaunchMode
                        ? new UranusGetDemoLaunchUrlGameApiRequest(
                            request.Game,
                            request.Language,
                            request.Device!,
                            PlayerIp: playerIp,
                            LobbyUrl: request.Lobby!)
                        : new UranusGetLaunchUrlGameApiRequest(
                            request.Game,
                            session.Id,
                            request.Language,
                            user.Currency.Id,
                            user.Id.ToString(),
                            request.CasinoId,
                            PlayerIp: playerIp);

                    var apiResponse = isDemoLaunchMode
                        ? await _uranusGameApiClient.GetDemoLaunchUrlAsync(
                            baseUrl,
                            apiApiRequest,
                            cancellationToken: cancellationToken)
                        : await _uranusGameApiClient.GetGameLaunchUrlAsync(
                            baseUrl,
                            apiApiRequest,
                            cancellationToken: cancellationToken);

                    if (IsInvalidUranusApiResponse(apiResponse))
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    launchUrl = apiResponse.Data.Data.Data.Url.ToString();
                    break;
                }

                case WalletProvider.Atlas:
                {
                    var isDemoLaunchMode = request.LaunchMode is LaunchMode.Demo;
                    const string stringToEncode = "atlas:password123";
                    var stringToEncodeAsBytes = Encoding.UTF8.GetBytes(stringToEncode);
                    var token = Convert.ToBase64String(stringToEncodeAsBytes);
                    var apiRequest = new AtlasGameLaunchGameApiRequest(
                        request.Game,
                        isDemoLaunchMode,
                        false,
                        session.Id,
                        request.CasinoId,
                        request.Language!,
                        request.Cashier!,
                        request.Lobby!);

                    var apiResponse = await _atlasGameApiClient.LaunchGameAsync(
                        baseUrl,
                        apiRequest,
                        token,
                        cancellationToken: cancellationToken);

                    if (apiResponse.IsFailure || apiResponse.Data?.Data?.Url is null)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    launchUrl = apiResponse.Data.Data.Url.ToString();
                    break;
                }

                case WalletProvider.EmaraPlay:
                {
                    var ip = GetPlayerIp();

                    var apiRequest = new EmaraplayGetLauncherUrlGameApiRequest(
                        request.CasinoId,
                        request.Game,
                        request.LaunchMode.ToString(),
                        request.Language!,
                        "someChannel",
                        "someJurisdiction",
                        user.Currency.Id,
                        ip!,
                        User: request.UserName,
                        Lobby: request.Lobby,
                        Cashier: "someCashier",
                        Token: session.Id);

                    var apiResponse = await _emaraPlayGameApiClient.GetLauncherUrlAsync(
                        baseUrl,
                        apiRequest,
                        cancellationToken: cancellationToken);

                    if (apiResponse.IsFailure || apiResponse.Data.Data.Result.Url is null)
                        ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    launchUrl = apiResponse.Data.Data.Result.Url?.ToString()!;
                    break;
                }

                case WalletProvider.Psw or WalletProvider.Betflag:
                {
                    var getGameLinkResult = await _pswGameApiClient.GameSessionAsync(
                        baseUrl,
                        user.Casino.Id,
                        session.Id,
                        user.Username,
                        user.Currency.Id,
                        request.Game,
                        request.LaunchMode,
                        request.PswRealityCheck,
                        casino.Provider is WalletProvider.Betflag,
                        cancellationToken: cancellationToken);

                    launchUrl = getGameLinkResult.Data?.Data.LaunchUrl ?? "";
                    break;
                }

                case WalletProvider.Openbox:
                    launchUrl = GetOpenboxLaunchUrl(
                        baseUrl,
                        session.Id,
                        user.CasinoId,
                        user.Id.ToString(),
                        user.Username,
                        request.Game,
                        user.CurrencyId);

                    break;

                case WalletProvider.Dafabet:
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

                case WalletProvider.Hub88:
                {
                    var getHub88GameLinkRequestDto = new Hub88GetLaunchUrlGameApiRequest(
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

                    var getGameLinkResult = await _hub88GameApiClient.GetLaunchUrlAsync(
                        baseUrl,
                        casino.Params.Hub88PrivateGameServiceSecuritySign,
                        getHub88GameLinkRequestDto,
                        cancellationToken);

                    if (getGameLinkResult.IsFailure || getGameLinkResult.Data.IsFailure)
                        launchUrl = "";
                    else
                        launchUrl = getGameLinkResult.Data.Data.Url;

                    break;
                }

                case WalletProvider.Softswiss:
                {
                    var getGameLinkResult = await _softswissGamesApiClient.GetLaunchUrlAsync(
                        baseUrl,
                        user.CasinoId,
                        user.Username,
                        session.Id,
                        game.GameServerId,
                        user.Currency.Id,
                        _currencyMultipliers.GetSumOut(user.Currency.Id, user.Balance),
                        cancellationToken);

                    if (getGameLinkResult.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    var data = getGameLinkResult.Data;

                    httpRequestMessage = data.HttpRequest;
                    httpResponseMessage = data.HttpResponse;

                    var content = data.Content!;
                    var existingSession = await _context.Set<Session>()
                       .Where(s => s.Id == content.SessionId)
                       .FirstOrDefaultAsync(cancellationToken);

                    if (existingSession is not null)
                    {
                        existingSession.ExpirationDate = session.ExpirationDate;
                        _context.Update(existingSession);
                        session = existingSession;
                    }
                    else
                    {
                        session.Id = content.SessionId;
                        _context.Add(session);
                    }

                    await _context.SaveChangesAsync(cancellationToken);

                    launchUrl = content.LaunchOptions.GameUrl;
                    break;
                }

                case WalletProvider.Sw:
                    launchUrl = GetSwLaunchUrl(
                        baseUrl,
                        session.Id,
                        $"{casino.Id}-{user.Currency.Id}",
                        user.Id.ToString(),
                        request.Game);

                    break;

                case WalletProvider.SoftBet:
                {
                    var launchMode = request.LaunchMode is LaunchMode.Real ? "real" : "fun";
                    var softBetGetLaunchUrlGameApiRequest = new SoftBetGetLaunchUrlGameApiRequest(
                        game.GameServerId,
                        casino.InternalId,
                        user.Username,
                        string.Empty,
                        casino.SignatureKey,
                        user.Username,
                        user.Currency.Id,
                        "ua",
                        1,
                        1,
                        launchMode,
                        "26",
                        request.Language,
                        "multi");

                    var getLaunchScriptResult = await _softBetGameApiClient.GetLaunchScriptAsync(
                        baseUrl,
                        softBetGetLaunchUrlGameApiRequest,
                        cancellationToken);

                    if (getLaunchScriptResult.IsFailure || getLaunchScriptResult.Data.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    launchUrl = ScriptHelper.ExtractUrlFromScript(getLaunchScriptResult.Data.Data, request.Environment);

                    break;
                }

                case WalletProvider.Uis:
                    launchUrl = GetUisLaunchUrl(
                        environment.UisBaseUrl,
                        session.Id,
                        casino.InternalId,
                        request.LaunchMode);

                    break;

                case WalletProvider.Reevo:
                    var reevoLaunchUrlResult = await _reevoGameApiClient.GetGameAsync(
                        baseUrl,
                        new ReevoGetGameGameApiRequest(
                            casino.Params.ReevoCallerId,
                            casino.Params.ReevoCallerPassword,
                            user.Id.ToString(),
                            user.Username,
                            request.Password,
                            "en",
                            game.GameServerId.ToString(),
                            request.Lobby ?? "",
                            request.LaunchMode is LaunchMode.Real ? "0" : "1",
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

                case WalletProvider.Everymatrix:
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

                case WalletProvider.BetConstruct:
                    launchUrl = GetBetConstructLaunchUrlAsync(
                        baseUrl,
                        game.GameServerId,
                        "en",
                        request.LaunchMode,
                        session.Id,
                        casino.Id);

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

            var result = new Response(
                session.Id,
                user.Balance,
                launchUrl,
                httpRequestMessage,
                httpResponseMessage);

            return ResultFactory.Success(result);
        }

        private string GetPlayerIp()
        {
            var playerIp = _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            return (!string.IsNullOrEmpty(playerIp)
                ? playerIp
                : _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString())!;
        }

        private static bool IsInvalidUranusApiResponse(IBaseResult? apiResponse)
        {
            return apiResponse is not null && apiResponse.IsFailure;
        }
    }

    private static string GetUisLaunchUrl(
        Uri baseUri,
        string token,
        int operatorId,
        LaunchMode launchType)
    {
        var queryParameters = new Dictionary<string, string?>();

        if (launchType is LaunchMode.Real or LaunchMode.Fun)
        {
            queryParameters.Add("token", token);
            queryParameters.Add("operatorID", operatorId.ToString());
        }

        if (launchType is LaunchMode.Fun or LaunchMode.Demo)
        {
            queryParameters.Add("demo", bool.TrueString);
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

    //TODO launchmode
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
            { "player-uid", playerUid },
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
            { nameof(currency), currency },
            { nameof(device), device ?? "desktop" }
        };

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

    private static string GetEmaraPlayLaunchUrlAsync(
        Uri baseUrl,
        string gameId,
        string token,
        LaunchMode launchMode,
        string lang,
        string @operator,
        string? channel,
        string? jurisdiction,
        string currency,
        string? ip,
        string user,
        string? lobby = null,
        string? cashier = null)
    {
        var mode = launchMode is LaunchMode.Real ? "real_play" : "demo";

        var queryParameters = new Dictionary<string, string?>
        {
            { nameof(mode), mode },
            { "gameId", gameId },
            { "lang", lang },
            { "operator", @operator },
            { nameof(channel), channel },
            { nameof(jurisdiction), jurisdiction },
            { nameof(ip), ip },
            { nameof(currency), currency },
        };

        if (mode is "real_play")
        {
            queryParameters.Add(nameof(token), token);
            queryParameters.Add(nameof(user), user);
        }

        if (lobby is not null)
        {
            queryParameters.Add(nameof(lobby), lobby);
        }

        if (cashier is not null)
        {
            queryParameters.Add(nameof(cashier), cashier);
        }

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(baseUrl, $"emara-play/launch{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetBetConstructLaunchUrlAsync(
        Uri baseUrl,
        int gameId,
        string language,
        LaunchMode launchMode,
        string session,
        string casinoId)
    {
        var mode = launchMode is LaunchMode.Real ? "real_play" : "demo";

        var queryParameters = new Dictionary<string, string?>
        {
            { "mode", mode },
            { "gameID", gameId.ToString() },
            { "language", language },
            { "partner", casinoId }
        };

        if (mode is "real_play")
            queryParameters.Add("token", session);

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(baseUrl, $"betconstruct{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetParimatchLaunchUrl(
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
        string LaunchUrl,
        string? HttpRequestMessage,
        string? HttpResponseMessage) : PswBalanceResponse(Balance);

    public class LoginRequestValidator : AbstractValidator<LogInRequest>
    {
        public LoginRequestValidator()
        {
        }
    }

    public class Validator : AbstractValidator<SignUpRequest>
    {
        public Validator(SupportedCurrenciesFactory supportedCurrenciesFactory)
        {
            var currenciesOptions = supportedCurrenciesFactory.Create();

            RuleFor(x => currenciesOptions.Items.Contains(x.Currency));

            RuleFor(p => p.Balance)
               .PrecisionScale(28, 2, false);
        }
    }
}