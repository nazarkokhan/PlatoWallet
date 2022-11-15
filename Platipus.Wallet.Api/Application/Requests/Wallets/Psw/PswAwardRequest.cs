namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;

public record PswAwardRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    decimal Amount,
    string AwardId) : PswBaseRequest(SessionId, User), IRequest<IPswResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<PswAwardRequest, IPswResult<PswBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<PswBalanceResponse>> Handle(
            PswAwardRequest request,
            CancellationToken cancellationToken)
        {
            var award = await _context.Set<Award>()
                .Where(a => a.Id == request.AwardId)
                .Include(a => a.User)
                .Include(a => a.AwardRound!.Round)
                .FirstOrDefaultAsync(cancellationToken);

            if (award is null || award.User.UserName != request.User)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.AwardDoesNotExist);

            if (award.AwardRound is not null)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.DuplicateTransaction);

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is not null)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.DuplicateTransaction);

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
                AwardRound = new AwardRound {Award = award}
            };
            _context.Add(round);

            var user = award.User;

            user.Rounds.Add(round);
            user.Balance += request.Amount;
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var result = new PswBalanceResponse(user.Balance);

            return PswResultFactory.Success(result);
        }
    }

    public class Validator : AbstractValidator<PswAwardRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .ScalePrecision(2, 38);
        }
    }
}