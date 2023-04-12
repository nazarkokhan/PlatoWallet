namespace Platipus.Wallet.Api.Application.Requests.External.Psw;

using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.Psw.Base.Response;
using Platipus.Wallet.Api.Application.Results.Psw;
using Platipus.Wallet.Api.Application.Services.PswGamesApi;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

public record PswCreateAwardRequest(
    string User,
    DateTime ValidUntil,
    string Game,
    string AwardId,
    int Count) : IRequest<IPswResult<PswBaseResponse>>
{
    public class Handler : IRequestHandler<PswCreateAwardRequest, IPswResult<PswBaseResponse>>
    {
        private readonly WalletDbContext _context;
        private readonly IPswAndBetflagGameApiClient _pswAndBetflagGameApiClient;

        public Handler(
            WalletDbContext context,
            IPswAndBetflagGameApiClient pswAndBetflagGameApiClient)
        {
            _context = context;
            _pswAndBetflagGameApiClient = pswAndBetflagGameApiClient;
        }

        public async Task<IPswResult<PswBaseResponse>> Handle(
            PswCreateAwardRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Username == request.User)
                .Include(
                    u => u.Awards
                        .Where(a => a.Id == request.AwardId))
                .Include(u => u.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return PswResultFactory.Failure<PswBaseResponse>(PswErrorCode.InvalidUser);

            if (user.Awards.Any(a => a.Id == request.AwardId))
                return PswResultFactory.Failure<PswBaseResponse>(PswErrorCode.DuplicateAward);

            var award = new Award(request.AwardId, request.ValidUntil);

            user.Awards.Add(award);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var createFreebetAwardResult = await _pswAndBetflagGameApiClient.CreateFreebetAwardAsync(
                user.CasinoId,
                user.Username,
                award.Id,
                user.Currency.Id,
                new[] { request.Game },
                request.ValidUntil,
                request.Count,
                cancellationToken);

            return createFreebetAwardResult;
        }
    }
}