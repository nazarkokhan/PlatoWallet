namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record PswWinRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    bool Finished,
    decimal Amount) : IPswBaseRequest, IRequest<IPswResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<PswWinRequest, IPswResult<PswBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<PswBalanceResponse>> Handle(
            PswWinRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId && r.User.UserName == request.User)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null || round.Transactions.Any(t => t.Id == request.TransactionId))
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.DuplicateTransaction);

            if (round.Finished)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.Unknown);

            if (round.User.Currency.Name != request.Currency)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.WrongCurrency);

            round.User.Balance += request.Amount;
            if (request.Finished)
                round.Finished = request.Finished;

            var transaction = new Transaction
            {
                Id = request.TransactionId,
                Amount = request.Amount
            };

            round.Transactions.Add(transaction);

            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new PswBalanceResponse(round.User.Balance);

            return PswResultFactory.Success(response);
        }
    }

    public class Validator : AbstractValidator<PswWinRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .ScalePrecision(2, 38);
        }
    }
}