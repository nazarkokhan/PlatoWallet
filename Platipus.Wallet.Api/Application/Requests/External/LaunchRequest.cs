namespace Platipus.Wallet.Api.Application.Requests.External;

using System.ComponentModel;
using System.Text;
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
using Services.BetconstructGameApi;
using Services.BetconstructGameApi.External;
using Services.DafabetGameApi;
using Services.DafabetGameApi.Requests;
using Services.EmaraPlayGameApi;
using Services.EmaraPlayGameApi.Requests;
using Services.EvenbetGameApi;
using Services.EvenbetGameApi.Requests;
using Services.EverymatrixGameApi;
using Services.EverymatrixGameApi.External;
using Services.Hub88GamesApi;
using Services.Hub88GamesApi.DTOs;
using Services.Hub88GamesApi.DTOs.Requests;
using Services.NemesisGameApi;
using Services.NemesisGameApi.Requests;
using Services.ObsoleteGameApiStyle.ReevoGamesApi;
using Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;
using Services.ObsoleteGameApiStyle.SoftswissGamesApi;
using Services.OpenboxGameApi;
using Services.OpenboxGameApi.External;
using Services.ParimatchGameApi;
using Services.ParimatchGameApi.Requests;
using Services.PswGameApi;
using Services.SoftBetGameApi;
using Services.SoftBetGameApi.External;
using Services.SwGameApi;
using Services.SwGameApi.Requests;
using Services.SynotGameApi;
using Services.SynotGameApi.Requests;
using Services.UisGamesApi;
using Services.UisGamesApi.Dto;
using Services.UranusGamesApi;
using Services.UranusGamesApi.Abstaction;
using Services.UranusGamesApi.Requests;
using Services.VegangsterGameApi;
using Services.VegangsterGameApi.External;
using StartupSettings.Factories;
using StartupSettings.Options;
using Wallets.Anakatech.Enums;

public sealed record LaunchRequest(
        [property: DefaultValue("753c57f7-234e-4112-925b-1f19b126682a")] string SessionToken,
        [property: DefaultValue("openbox")] string CasinoId,
        [property: DefaultValue("extragems")] string Game,
        [property: DefaultValue("test")] string Environment,
        [property: DefaultValue("some_lobby_url")] string Lobby,
        [property: DefaultValue(LaunchMode.Demo)] LaunchMode LaunchMode,
        [property: DefaultValue("en")] string Language)
    : IRequest<IResult<LaunchRequest.Response>>
{
    public sealed class Handler : IRequestHandler<LaunchRequest, IResult<Response>>
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
        private readonly IDafabetGameApiClient _dafabetGameApiClient;
        private readonly IEverymatrixGameApiClient _everymatrixGameApiClient;
        private readonly IBetconstructGameApiClient _betconstructGameApiClient;
        private readonly IOpenboxGameApiClient _openboxGameApiClient;
        private readonly ISwGameApiClient _swGameApiClient;
        private readonly IUisGameApiClient _uisGameApiClient;

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
            ISoftBetGameApiClient softBetGameApiClient,
            IDafabetGameApiClient dafabetGameApiClient,
            IEverymatrixGameApiClient everymatrixGameApiClient,
            IBetconstructGameApiClient betconstructGameApiClient,
            IOpenboxGameApiClient openboxGameApiClient,
            ISwGameApiClient swGameApiClient,
            IUisGameApiClient uisGameApiClient)
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
            _dafabetGameApiClient = dafabetGameApiClient;
            _everymatrixGameApiClient = everymatrixGameApiClient;
            _betconstructGameApiClient = betconstructGameApiClient;
            _openboxGameApiClient = openboxGameApiClient;
            _swGameApiClient = swGameApiClient;
            _uisGameApiClient = uisGameApiClient;
            _currencyMultipliers = currencyMultipliers.Value;
        }

        public async Task<IResult<Response>> Handle(LaunchRequest request, CancellationToken cancellationToken)
        {
            const string defaultCountry = "UA";
            const string defaultPlatform = "desktop";

            var casino = await _context.Set<Casino>()
               .Where(c => c.Id == request.CasinoId)
               .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                return ResultFactory.Failure<Response>(ErrorCode.CasinoNotFound);

            var userId = await _context.Set<Session>()
               .Where(s => s.Id == request.SessionToken)
               .Select(u => u.UserId)
               .FirstOrDefaultAsync(cancellationToken);

            var user = await _context.Set<User>()
               .Where(u => u.Id == userId && u.CasinoId == request.CasinoId)
               .Include(u => u.Casino)
               .Include(u => u.Currency)
               .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<Response>(ErrorCode.UserNotFound);

            if (user.IsDisabled)
                return ResultFactory.Failure<Response>(ErrorCode.UserIsDisabled);

            var session = await _context.Set<Session>()
               .Where(s => s.Id == request.SessionToken)
               .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return ResultFactory.Failure<Response>(ErrorCode.SessionNotFound);

            if (session.IsTemporaryToken && session.ExpirationDate < DateTime.UtcNow)
                return ResultFactory.Failure<Response>(ErrorCode.SessionExpired);

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

            var baseUrl = environment.BaseUrl;

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
                            request.CasinoId,
                            request.Game,
                            defaultPlatform,
                            user.CurrencyId,
                            LobbyUrl: request.Lobby,
                            Lang: request.Language,
                            Country: defaultCountry,
                            Ip: playerIp)
                        : new VegangsterGetLaunchUrlGameApiRequest(
                            request.CasinoId,
                            user.Id.ToString(),
                            session.Id,
                            request.Game,
                            defaultPlatform,
                            user.CurrencyId,
                            request.Language,
                            defaultCountry,
                            playerIp,
                            request.Lobby,
                            "some_deposit_url",
                            user.Username);

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
                        "desktop",
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
                        user.Id.ToString(),
                        casino.Id,
                        session.Id,
                        "some securityToken",
                        user.Id.ToString(),
                        request.Game,
                        playMode,
                        user.Username,
                        (int)user.Balance,
                        user.Currency.Id,
                        request.Language,
                        defaultCountry,
                        request.Lobby,
                        "some jurisdiction",
                        "some origin url",
                        123);

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
                        defaultPlatform,
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
                            defaultPlatform,
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
                        request.Language,
                        "some cashier url",
                        request.Lobby);

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
                        User: user.Username,
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
                        123,
                        casino.Provider is WalletProvider.Betflag,
                        cancellationToken: cancellationToken);

                    launchUrl = getGameLinkResult.Data?.Data.LaunchUrl ?? "";
                    break;
                }

                case WalletProvider.Openbox:
                {
                    var playerType = request.LaunchMode is LaunchMode.Real ? 1 : 3;
                    var getOpenboxLaunchScriptGameApiRequest = new OpenboxGetLaunchScriptGameApiRequest(
                        session.Id,
                        user.CasinoId,
                        user.Id.ToString(),
                        user.Username,
                        request.Game,
                        user.CurrencyId,
                        playerType,
                        defaultCountry,
                        request.Language,
                        request.Lobby);

                    var getLaunchScriptResult = await _openboxGameApiClient.GetLaunchScriptAsync(
                        baseUrl,
                        getOpenboxLaunchScriptGameApiRequest,
                        cancellationToken);

                    if (getLaunchScriptResult.IsFailure || getLaunchScriptResult.Data.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    launchUrl = ScriptHelper.ExtractUrlFromScript(getLaunchScriptResult.Data.Data, request.Environment);

                    break;
                }

                case WalletProvider.Dafabet:
                {
                    var getDafabetLaunchUrlGameApiRequest = new DafabetGetLaunchUrlGameApiRequest(
                        casino.Id,
                        request.Game,
                        user.Username,
                        session.Id,
                        user.Currency.Id,
                        defaultPlatform,
                        request.Language,
                        request.Lobby);

                    var getLaunchScriptResult = await _dafabetGameApiClient.GetLaunchScriptAsync(
                        baseUrl,
                        casino.SignatureKey,
                        getDafabetLaunchUrlGameApiRequest,
                        cancellationToken);

                    if (getLaunchScriptResult.IsFailure || getLaunchScriptResult.Data.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    launchUrl = ScriptHelper.ExtractUrlFromScript(getLaunchScriptResult.Data.Data, request.Environment);

                    break;
                }

                case WalletProvider.Hub88:
                {
                    var getHub88GameLinkRequestDto = new Hub88GetLaunchUrlGameApiRequest(
                        user.Username,
                        session.Id,
                        user.CasinoId,
                        defaultPlatform,
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
                {
                    var key = $"{casino.Id}-{user.Currency.Id}";
                    var swGetLaunchUrlGameApiRequest = new SwGetLaunchUrlGameApiRequest(
                        session.Id,
                        key,
                        user.Id.ToString(),
                        request.Game,
                        request.Language);

                    var getLaunchScriptResult = await _swGameApiClient.GetLaunchScriptAsync(
                        baseUrl,
                        swGetLaunchUrlGameApiRequest,
                        cancellationToken);

                    if (getLaunchScriptResult.IsFailure || getLaunchScriptResult.Data.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    launchUrl = ScriptHelper.ExtractUrlFromScript(getLaunchScriptResult.Data.Data, request.Environment);

                    break;
                }

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
                {
                    var ip = GetPlayerIp();
                    var uisGetLaunchUrlGameApiRequest = new UisGetLaunchGameApiRequest(
                        session.Id,
                        request.Game,
                        string.Empty,
                        "150550",
                        ip,
                        request.Lobby,
                        request.Language);

                    var getLaunchScriptResult = await _uisGameApiClient.GetLaunchScriptAsync(
                        baseUrl,
                        uisGetLaunchUrlGameApiRequest,
                        cancellationToken);

                    if (getLaunchScriptResult.IsFailure || getLaunchScriptResult.Data.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    httpRequestMessage = getLaunchScriptResult.Data.HttpRequest.RequestData.RequestUri.AbsoluteUri;

                    launchUrl = ScriptHelper.ExtractUrlFromScript(getLaunchScriptResult.Data.Data, request.Environment);

                    break;
                }

                case WalletProvider.Reevo:
                    var reevoLaunchUrlResult = await _reevoGameApiClient.GetGameAsync(
                        baseUrl,
                        new ReevoGetGameGameApiRequest(
                            casino.Params.ReevoCallerId,
                            casino.Params.ReevoCallerPassword,
                            user.Id.ToString(),
                            user.Username,
                            user.Password,
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
                {
                    var isFreePlay = request.LaunchMode is LaunchMode.Demo;
                    var isMobile = string.Equals(defaultPlatform, "mobile", StringComparison.OrdinalIgnoreCase);
                    var getLaunchUrlGameApiRequest = new EverymatrixGetLaunchUrlGameApiRequest(
                        casino.Id,
                        request.Game,
                        request.Language,
                        isFreePlay,
                        isMobile,
                        "dev",
                        session.Id,
                        request.Lobby,
                        user.Currency.Id);

                    var getLaunchScriptResult = await _everymatrixGameApiClient.GetLaunchScriptAsync(
                        baseUrl,
                        getLaunchUrlGameApiRequest,
                        cancellationToken);

                    if (getLaunchScriptResult.IsFailure || getLaunchScriptResult.Data.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    launchUrl = ScriptHelper.ExtractUrlFromScript(getLaunchScriptResult.Data.Data, request.Environment);

                    break;
                }

                case WalletProvider.BetConstruct:
                {
                    var mode = request.LaunchMode is LaunchMode.Real ? "real_play" : "demo";
                    var getLaunchScriptGameApiRequest = new BetconstructGetLaunchScriptGameApiRequest(
                        game.GameServerId,
                        request.Language,
                        mode,
                        string.Equals(mode, "real_play", StringComparison.OrdinalIgnoreCase) ? session.Id : null,
                        casino.Id);

                    var getLaunchScriptResult = await _betconstructGameApiClient.GetLaunchScriptAsync(
                        baseUrl,
                        getLaunchScriptGameApiRequest,
                        cancellationToken);

                    if (getLaunchScriptResult.IsFailure || getLaunchScriptResult.Data.IsFailure)
                        return ResultFactory.Failure<Response>(ErrorCode.GameServerApiError);

                    launchUrl = ScriptHelper.ExtractUrlFromScript(getLaunchScriptResult.Data.Data, request.Environment);

                    break;
                }

                default:
                    launchUrl = "";
                    break;
            }

            if (!string.IsNullOrWhiteSpace(launchUrl) && !launchUrl.StartsWith("<"))
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

    public sealed record Response(
        string SessionId,
        decimal Balance,
        string LaunchUrl,
        string? HttpRequestMessage,
        string? HttpResponseMessage);

    public class LaunchRequestValidator : AbstractValidator<LaunchRequest>
    {
        public LaunchRequestValidator()
        {
        }
    }

    public sealed class Validator : AbstractValidator<SignUpRequest>
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