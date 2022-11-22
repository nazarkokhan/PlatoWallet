namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Common;

public record SetCasinoGamesRequest(string CasinoId, List<string> GameLaunchNames) : IRequest<IResult>
{
    public class Handler : IRequestHandler<SetCasinoGamesRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            SetCasinoGamesRequest request,
            CancellationToken cancellationToken)
        {
            var casino = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .Include(c => c.CasinoGames)
                .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                return ResultFactory.Failure(ErrorCode.InvalidCasinoId);

            var games = await _context.Set<Game>()
                .Where(g => request.GameLaunchNames.Contains(g.LaunchName))
                .Select(g => g.Id)
                .ToListAsync(cancellationToken);

            var casinoGamesToAdd = games.Select(g => new CasinoGames {GameId = g}).ToList();
            casino.CasinoGames = casinoGamesToAdd;
            // casinoGamesToAdd.AddRange(casinoGamesToAdd);

            _context.Update(casino);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }

    // public class Validator : AbstractValidator<SetCasinoGamesRequest>
    // {
    //     public Validator()
    //     {
    //         RuleFor(p => p.Balance).ScalePrecision(2, 38);
    //     }
    // }
}