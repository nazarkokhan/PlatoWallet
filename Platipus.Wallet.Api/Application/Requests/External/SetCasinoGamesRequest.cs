namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
                return ResultFactory.Failure(ErrorCode.CasinoNotFound);

            var games = await _context.Set<Game>()
                .Where(g => request.GameLaunchNames.Contains(g.LaunchName))
                .Select(g => g.Id)
                .ToListAsync(cancellationToken);
            _context.RemoveRange(casino.CasinoGames);

            var casinoGamesToAdd = games
                .Select(g => new CasinoGames { GameId = g })
                .ToList();

            _context.AddRange(casinoGamesToAdd);
            casino.CasinoGames.AddRange(casinoGamesToAdd);

            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}