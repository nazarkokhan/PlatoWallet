namespace PlatipusWallet.Api.Application.Requests.Wallet;

using Base.Requests;
using Base.Responses;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record WinRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    bool Finished,
    decimal Amount) : BaseRequest(SessionId), IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<WinRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BalanceResponse>> Handle(
            WinRequest request,
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

            if (round.Transactions.Any(t => t.Id == request.TransactionId))
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

            if (round.Finished)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.Unknown);

            if (round.User.Currency.Name != request.Currency)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);

            round.User.Balance += request.Amount;
            if (request.Finished)
                round.Finished = request.Finished;

            var transaction = new Transaction
            {
                Id = request.TransactionId,
                Amount = request.Amount,
                TransactionType = TransactionType.Win
            };

            round.Transactions.Add(transaction);

            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new BalanceResponse(round.User.Balance);

            return ResultFactory.Success(response);
        }
    }
}