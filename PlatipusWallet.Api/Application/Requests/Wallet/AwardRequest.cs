namespace PlatipusWallet.Api.Application.Requests.Wallet;

using Base.Requests;
using Base.Responses;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record AwardRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    decimal Amount,
    string AwardId) : BaseRequest(SessionId), IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<AwardRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BalanceResponse>> Handle(
            AwardRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(r => r.UserName == request.User)
                .Include(r => r.Currency)
                .Include(r => r.Awards)
                .Include(r => r.Rounds)
                .ThenInclude(r => r.Transactions)
                .FirstAsync(cancellationToken);
            //TODO Award hierarchy dependencies
            
            if (user.Currency.Name != request.Currency)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);
            
            if (user.Awards.Any(a => a.Id == request.AwardId))
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateAward);

            var award = new Award
            {
                Id = request.AwardId,
                Amount = request.Amount
            };
            
            user.Awards.Add(award);

            _context.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            var result = new BalanceResponse(user.Balance);

            return ResultFactory.Success(result);
        }
    }
}