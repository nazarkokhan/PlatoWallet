namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Infrastructure.Persistence;
using EntityFrameworkQueryableExtensions = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;

public record CreateGameRequest(
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
            var game = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                _context.Set<Game>()
                    .Where(c => c.GameServiceId == request.GameServerId),
                cancellationToken);

            if (game is not null)
                return ResultFactory.Failure(ErrorCode.GameNotFound);

            game = new Game
            {
                GameServiceId = request.GameServerId,
                Name = request.Name,
                LaunchName = request.LaunchName,
                CategoryId = request.CategoryId
            };

            _context.Add(game);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}