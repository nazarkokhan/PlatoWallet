namespace Platipus.Wallet.Api.Application.Requests.External.Softswiss;

using System.Text.Json.Serialization;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.ObsoleteGameApiStyle.SoftswissGamesApi;
using Services.Wallet;
using StartupSettings.Options;

public sealed record ExternalSoftswissGetLaunchUrlRequest(
        [property: JsonPropertyName("sessionToken")] string SessionToken,
        [property: JsonPropertyName("gameId")] int GameId,
        [property: JsonPropertyName("casinoId")] string CasinoId,
        string Environment)
    : IRequest<IResult<string>>
{
    public sealed class Handler : IRequestHandler<ExternalSoftswissGetLaunchUrlRequest, IResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly ISoftswissGamesApiClient _softswissGamesApiClient;
        private readonly SoftswissCurrenciesOptions _currencyMultipliers;
        private readonly WalletDbContext _walletDbContext;

        public Handler(
            IWalletService walletService,
            ISoftswissGamesApiClient softswissGamesApiClient,
            IOptions<SoftswissCurrenciesOptions> currencyMultipliers,
            WalletDbContext walletDbContext)
        {
            _walletService = walletService;
            _softswissGamesApiClient = softswissGamesApiClient;
            _walletDbContext = walletDbContext;
            _currencyMultipliers = currencyMultipliers.Value;
        }

        public async Task<IResult<string>> Handle(
            ExternalSoftswissGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var casino = await _walletDbContext.Set<Casino>()
               .Where(c => c.Id == request.CasinoId)
               .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                return ResultFactory.Failure<string>(ErrorCode.CasinoNotFound);

            var userId = await _walletDbContext.Set<Session>()
               .Where(s => s.Id == request.SessionToken)
               .Select(u => u.UserId)
               .FirstOrDefaultAsync(cancellationToken);

            var user = await _walletDbContext.Set<User>()
               .Where(u => u.Id == userId && u.CasinoId == request.CasinoId)
               .Include(u => u.Casino)
               .Include(u => u.Currency)
               .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<string>(ErrorCode.UserNotFound);

            if (user.IsDisabled)
                return ResultFactory.Failure<string>(ErrorCode.UserIsDisabled);

            var session = await _walletDbContext.Set<Session>()
               .Where(s => s.Id == request.SessionToken)
               .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return ResultFactory.Failure<string>(ErrorCode.SessionNotFound);

            if (session.IsTemporaryToken && session.ExpirationDate < DateTime.UtcNow)
                return ResultFactory.Failure<string>(ErrorCode.SessionExpired);

            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var getGameLinkResult = await _softswissGamesApiClient.GetLaunchUrlAsync(
                walletResponse.Data.BaseUrl,
                user.CasinoId,
                user.Username,
                session.Id,
                request.GameId,
                user.Currency.Id,
                _currencyMultipliers.GetSumOut(user.Currency.Id, user.Balance),
                cancellationToken);

            if (getGameLinkResult.IsFailure)
                return ResultFactory.Failure<string>(ErrorCode.GameServerApiError);

            var data = getGameLinkResult.Data;

            var content = data.Content!;
            var existingSession = await _walletDbContext.Set<Session>()
               .Where(s => s.Id == content.SessionId)
               .FirstOrDefaultAsync(cancellationToken);

            if (existingSession is not null)
            {
                existingSession.ExpirationDate = session.ExpirationDate;
                _walletDbContext.Update(existingSession);
            }
            else
            {
                session.Id = content.SessionId;
                _walletDbContext.Add(session);
            }

            await _walletDbContext.SaveChangesAsync(cancellationToken);

            var launchUrl = content.LaunchOptions.GameUrl;

            return ResultFactory.Success(launchUrl);
        }
    }
}