namespace PlatipusWallet.Api.Application.Requests.Wallet;

using Base.Requests;
using Base.Responses;
using Domain.Entities;
using FluentValidation;
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
            var award = await _context.Set<Award>()
                .Where(a => a.Id == request.AwardId)
                .Include(a => a.User)
                .Include(a => a.AwardRound!.Round)
                .FirstOrDefaultAsync(cancellationToken);

            if (award is null || award.User.UserName != request.User)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.AwardDoesNotExist);

            if (award.AwardRound is not null)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is not null)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

            round = new Round
            {
                Id = request.RoundId,
                Finished = true,
                Transactions = new List<Transaction>
                {
                    new()
                    {
                        Id = request.TransactionId,
                        Amount = request.Amount
                    }
                },
                AwardRound = new AwardRound
                {
                    Award = award
                }
            };
            _context.Add(round);

            var user = award.User;

            user.Rounds.Add(round);
            user.Balance += request.Amount;
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var result = new BalanceResponse(user.Balance);

            return ResultFactory.Success(result);
        }
    }
    
    public class Validator : AbstractValidator<AwardRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .ScalePrecision(38, 2);
        }
    }
}