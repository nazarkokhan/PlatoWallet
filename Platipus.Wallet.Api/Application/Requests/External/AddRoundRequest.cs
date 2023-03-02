namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using DTO;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record AddRoundRequest(
    string User,
    string RoundId) : IRequest<IResult>
{
    public class Handler : IRequestHandler<AddRoundRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            AddRoundRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Username == request.User)
                .Include(u => u.Rounds)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.UserNotFound);

            if (user.Rounds.Any(r => r.Id == request.RoundId))
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.BadParametersInTheRequest);

            var round = new Round(request.RoundId) { UserId = user.Id };
            _context.Add(round);

            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}