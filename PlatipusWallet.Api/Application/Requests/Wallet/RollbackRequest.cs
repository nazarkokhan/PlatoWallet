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

public record RollbackRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    decimal Amount) : BaseRequest(SessionId), IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<RollbackRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BalanceResponse>> Handle(
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
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.BadParametersInTheRequest);

            if (round.Finished)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.Unknown);

            var lastTransaction = round.Transactions.MaxBy(t => t.CreatedDate);
            if (lastTransaction is null || lastTransaction.Id != request.TransactionId)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.TransactionDoesNotExist);

            var user = round.User;
            if (user.Currency.Name != request.Currency)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);

            user.Balance += request.Amount;
            _context.Update(user);

            round.Transactions.Remove(lastTransaction);
            _context.Update(round);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new BalanceResponse(user.Balance);

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