namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DeleteGameRequest(int Id) : IRequest<IResult>
{
    public class Handler : IRequestHandler<DeleteGameRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            DeleteGameRequest request,
            CancellationToken cancellationToken)
        {
            var deletedGames = await _context.Set<Game>()
                .Where(c => c.GameServiceId == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedGames is 0)
                return ResultFactory.Failure(ErrorCode.GameNotFound);

            return ResultFactory.Success();
        }
    }
}