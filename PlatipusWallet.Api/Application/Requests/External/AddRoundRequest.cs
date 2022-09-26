namespace PlatipusWallet.Api.Application.Requests.External;

using Base.Responses;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record AddRoundRequest(
    string User,
    string RoundId) : IRequest<IResult<BaseResponse>>
{
    public class Handler : IRequestHandler<AddRoundRequest, IResult<BaseResponse>>
    {
        private readonly WalletDbContext _context;
        
        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BaseResponse>> Handle(
            AddRoundRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.User)
                .Include(u => u.Rounds)
                .FirstOrDefaultAsync(cancellationToken);
            

            if (user is null)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.InvalidUser);
            
            if (user.Rounds.Any(r => r.Id == request.RoundId))
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.BadParametersInTheRequest);

            var round = new Round
            {
                Id = request.RoundId,
            };
            user.Rounds.Add(round);
            
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);
            
            var result = new BalanceResponse(user.Balance);

            return ResultFactory.Success(result);
        }
    }
}