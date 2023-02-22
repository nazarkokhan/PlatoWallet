namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record UpdateGameRequest(
    int GameServerId,
    string Name,
    string LaunchName,
    int CategoryId) : IRequest<IResult>
{
    public class Handler : IRequestHandler<CreateGameRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            CreateGameRequest request,
            CancellationToken cancellationToken)
        {
            var game = await _context.Set<Game>()
                .Where(c => c.GameServerId == request.GameServerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (game is null)
                return ResultFactory.Failure(ErrorCode.InvalidGame);

            game.GameServerId = request.GameServerId;
            game.Name = request.Name;
            game.LaunchName = request.LaunchName;
            game.CategoryId = request.CategoryId;

            _context.Update(game);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}