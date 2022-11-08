namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Psw;
using Results.Psw.WithData;

public record RollbackRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    decimal Amount) : PswBaseRequest(SessionId, User), IRequest<IResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<RollbackRequest, IResult<PswBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<PswBalanceResponse>> Handle(
            RollbackRequest request,
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
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.BadParametersInTheRequest);

            if (round.Finished)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.Unknown);

            var lastTransaction = round.Transactions.MaxBy(t => t.CreatedDate);
            if (lastTransaction is null || lastTransaction.Id != request.TransactionId)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.TransactionDoesNotExist);

            var user = round.User;
            if (user.Currency.Name != request.Currency)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.WrongCurrency);

            user.Balance += request.Amount;
            _context.Update(user);

            round.Transactions.Remove(lastTransaction);
            _context.Update(round);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new PswBalanceResponse(user.Balance);

            return ResultFactory.Success(response);
        }
    }

    public class Validator : AbstractValidator<RollbackRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .ScalePrecision(2, 38);
        }
    }
}