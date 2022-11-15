namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;

public record PswRollbackRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    decimal Amount) : PswBaseRequest(SessionId, User), IRequest<IPswResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<PswRollbackRequest, IPswResult<PswBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<PswBalanceResponse>> Handle(
            PswRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId && r.User.UserName == request.User)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.BadParametersInTheRequest);

            if (round.Finished)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.Unknown);

            var lastTransaction = round.Transactions.MaxBy(t => t.CreatedDate);
            if (lastTransaction is null || lastTransaction.Id != request.TransactionId)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.TransactionDoesNotExist);

            var user = round.User;
            if (user.Currency.Name != request.Currency)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.WrongCurrency);

            user.Balance += request.Amount;
            _context.Update(user);

            round.Transactions.Remove(lastTransaction);
            _context.Update(round);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new PswBalanceResponse(user.Balance);

            return PswResultFactory.Success(response);
        }
    }

    public class Validator : AbstractValidator<PswRollbackRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .ScalePrecision(2, 38);
        }
    }
}