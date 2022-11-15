namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;
using Wallets.Psw.Base.Response;

public record AddRoundRequest(
    string User,
    string RoundId) : IRequest<IPswResult<PswBaseResponse>>
{
    public class Handler : IRequestHandler<AddRoundRequest, IPswResult<PswBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<PswBaseResponse>> Handle(
            AddRoundRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.User)
                .Include(u => u.Rounds)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.InvalidUser);

            if (user.Rounds.Any(r => r.Id == request.RoundId))
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.BadParametersInTheRequest);

            var round = new Round
            {
                Id = request.RoundId,
            };
            user.Rounds.Add(round);

            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var result = new PswBalanceResponse(user.Balance);

            return PswResultFactory.Success(result);
        }
    }
}