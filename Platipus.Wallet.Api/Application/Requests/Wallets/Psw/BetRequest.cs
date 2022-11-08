namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Psw;
using Results.Psw.WithData;

public record BetRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    bool Finished,
    decimal Amount) : PswBaseRequest(SessionId, User), IRequest<IResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<BetRequest, IResult<PswBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<PswBalanceResponse>> Handle(
            BetRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(
                    r => r.Id == request.RoundId &&
                         r.User.UserName == request.User)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                var user = await _context.Set<User>()
                    .Where(u => u.UserName == request.User)
                    .Include(u => u.Currency)
                    .FirstAsync(cancellationToken);

                round = new Round
                {
                    Id = request.RoundId,
                    Finished = false,
                    User = user
                };
                _context.Add(round);

                await _context.SaveChangesAsync(cancellationToken);
            }

            if (round.Transactions.Any(t => t.Id == request.TransactionId))
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.DuplicateTransaction);

            if (round.Finished)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.Unknown);

            if (round.User.Currency.Name != request.Currency)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.WrongCurrency);

            round.User.Balance -= request.Amount;
            if (request.Finished)
                round.Finished = request.Finished;

            var transaction = new Transaction
            {
                Id = request.TransactionId,
                Amount = request.Amount,
            };

            round.Transactions.Add(transaction);

            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new PswBalanceResponse(round.User.Balance);

            return ResultFactory.Success(response);
        }
    }
    
    public class Validator : AbstractValidator<BetRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .ScalePrecision(2, 38);
        }
    }
}