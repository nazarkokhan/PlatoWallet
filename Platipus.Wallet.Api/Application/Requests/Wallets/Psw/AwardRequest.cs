namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Psw;
using Results.Psw.WithData;

public record AwardRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    decimal Amount,
    string AwardId) : PswBaseRequest(SessionId, User), IRequest<IResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<AwardRequest, IResult<PswBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<PswBalanceResponse>> Handle(
            AwardRequest request,
            CancellationToken cancellationToken)
        {
            var award = await _context.Set<Award>()
                .Where(a => a.Id == request.AwardId)
                .Include(a => a.User)
                .Include(a => a.AwardRound!.Round)
                .FirstOrDefaultAsync(cancellationToken);

            if (award is null || award.User.UserName != request.User)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.AwardDoesNotExist);

            if (award.AwardRound is not null)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.DuplicateTransaction);

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is not null)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.DuplicateTransaction);

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

            var result = new PswBalanceResponse(user.Balance);

            return ResultFactory.Success(result);
        }
    }
    
    public class Validator : AbstractValidator<AwardRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .ScalePrecision(2, 38);
        }
    }
}