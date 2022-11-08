namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Results.Psw;
using Results.Psw.WithData;
using Wallets.Psw.Base.Response;

public record AddRoundRequest(
    string User,
    string RoundId) : IRequest<IResult<PswBaseResponse>>
{
    public class Handler : IRequestHandler<AddRoundRequest, IResult<PswBaseResponse>>
    {
        private readonly WalletDbContext _context;
        
        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<PswBaseResponse>> Handle(
            AddRoundRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.User)
                .Include(u => u.Rounds)
                .FirstOrDefaultAsync(cancellationToken);
            

            if (user is null)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.InvalidUser);
            
            if (user.Rounds.Any(r => r.Id == request.RoundId))
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.BadParametersInTheRequest);

            var round = new Round
            {
                Id = request.RoundId,
            };
            user.Rounds.Add(round);
            
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);
            
            var result = new PswBalanceResponse(user.Balance);

            return ResultFactory.Success(result);
        }
    }
}