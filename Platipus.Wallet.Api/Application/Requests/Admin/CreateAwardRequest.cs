namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Psw;
using Results.Psw.WithData;
using Services.GamesApi;
using Wallets.Psw.Base.Response;

public record CreateAwardRequest(
    //TODO by user or session? Guid SessionId,
    string User,
    DateTime ValidUntil,
    string Game,
    string AwardId) : IRequest<IResult<PswBaseResponse>>
{
    public class Handler : IRequestHandler<CreateAwardRequest, IResult<PswBaseResponse>>
    {
        private readonly WalletDbContext _context;
        private readonly IGamesApiClient _gamesApiClient;

        public Handler(
            WalletDbContext context,
            IGamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<IResult<PswBaseResponse>> Handle(
            CreateAwardRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.User)
                .Include(u => u.Awards
                    .Where(a => a.Id == request.AwardId))
                .Include(u => u.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<PswBaseResponse>(ErrorCode.InvalidUser);

            if (user.Awards.Any(a => a.Id == request.AwardId))
                return ResultFactory.Failure<PswBaseResponse>(ErrorCode.DuplicateAward);

            var award = new Award
            {
                Id = request.AwardId,
                ValidUntil = request.ValidUntil
            };

            user.Awards.Add(award);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var createFreebetAwardResult = await _gamesApiClient.CreateFreebetAwardAsync(
                user.CasinoId,
                user.UserName,
                award.Id,
                user.Currency.Name,
                new[] {request.Game},
                request.ValidUntil,
                10, //TODO where to get from?
                cancellationToken);

            return createFreebetAwardResult;
        }
    }
}