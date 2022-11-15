namespace Platipus.Wallet.Api.Application.Requests.External;

using Api.Extensions.SecuritySign;
using Base;
using Domain.Entities;
using Domain.Entities.Enums;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Results.Psw;
using Services.GamesApi;
using StartupSettings.Options;
using Wallets.Psw.Base.Response;

public record LogInRequest(
    string UserName,
    string Password,
    string CasinoId,
    string Game,
    string? Device) : BaseRequest, IRequest<IPswResult<LogInRequest.Response>>
{
    public class Handler : IRequestHandler<LogInRequest, IPswResult<Response>>
    {
        private readonly WalletDbContext _context;
        private readonly IGamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, IGamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
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

            string launchUrl;

            //TODO refactor
            if (casino.Provider is CasinoProvider.Openbox)
            {
                launchUrl = GetOpenboxLaunchUrl(
                    session.Id,
                    user.CasinoId,
                    user.Id,
                    user.UserName,
                    request.Game,
                    user.Currency.Name);
            }
            else if (casino.Provider is CasinoProvider.Dafabet)
            {
                launchUrl = GetDafabetLaunchUrl(
                    request.Game,
                    user.UserName,
                    session.Id,
                    user.Currency.Name,
                    request.Device,
                    "en",
                    DatabetHash.Compute(
                        $"launch{request.Game}{user.UserName}{session.Id}{user.Currency.Name}",
                        casino.SignatureKey));
            }
            else
            {
                var getGameLinkResult = await _gamesApiClient.GetGameLinkAsync(
                    user.Casino.Id,
                    session.Id,
                    user.UserName,
                    user.Currency.Name,
                    request.Game,
                    cancellationToken: cancellationToken);

                launchUrl = getGameLinkResult.Data?.LaunchUrl ?? "";
            }

            var result = new Response(session.Id, user.Balance, launchUrl);

            return PswResultFactory.Success(result);
        }
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
            {nameof(token), token.ToString()},
            {"agency-uid", agencyUid},
            {"player-uid", playerUid.ToString()},
            {"player-type", "1"},
            {"player-id", playerId},
            {"game-id", gameId},
            {"country", "CN"},
            {"language", "en"},
            {nameof(currency), currency},
            // {"backurl", "zero"},
            // {"backUri", "zero"},
        };

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(
            new Uri("https://test.platipusgaming.com/onlinecasino/"),
            $"openbox/launcher{queryString.ToUriComponent()}");

        return uri.AbsoluteUri;
    }

    private static string GetDafabetLaunchUrl(
        string gameCode,
        string playerId,
        Guid playerToken,
        string currency,
        string device,
        string? language,
        string hash)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            {"brand", "dafabet"},
            {nameof(gameCode), gameCode},
            {nameof(playerId), playerId},
            {nameof(playerToken), playerToken.ToString()},
            {nameof(currency), currency},
            {nameof(device), device},
        };

        if (language is not null)
            queryParameters.Add(nameof(language), language);

        queryParameters.Add(nameof(hash), hash);

        var queryString = QueryString.Create(queryParameters);

        var uri = new Uri(new Uri("https://test.platipusgaming.com/"), $"dafabet/launch{queryString.ToUriComponent()}");

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