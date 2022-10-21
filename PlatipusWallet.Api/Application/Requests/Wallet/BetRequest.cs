namespace PlatipusWallet.Api.Application.Requests.Wallet;

using Base.Requests;
using Base.Responses;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result.Factories;
using Results.Common.Result.WithData;

public record BetRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    bool Finished,
    decimal Amount) : BaseRequest(SessionId, User), IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<BetRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BalanceResponse>> Handle(
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
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

            if (round.Finished)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.Unknown);

            if (round.User.Currency.Name != request.Currency)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);

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

            var response = new BalanceResponse(round.User.Balance);

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