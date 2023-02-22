namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Infrastructure.Persistence;
using EntityFrameworkQueryableExtensions = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;

public record AddGameRequest(
    int GameServerId,
    string Name,
    string LaunchName,
    int CategoryId) : IRequest<IResult>
{
    public class Handler : IRequestHandler<AddGameRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            AddGameRequest request,
            CancellationToken cancellationToken)
        {
            var game = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                _context.Set<Game>()
                    .Where(c => c.GameServerId == request.GameServerId),
                cancellationToken);

            if (game is not null)
                return ResultFactory.Failure(ErrorCode.InvalidGame);

            game = new Game
            {
                GameServerId = request.GameServerId,
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