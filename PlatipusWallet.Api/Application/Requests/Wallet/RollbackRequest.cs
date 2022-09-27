namespace PlatipusWallet.Api.Application.Requests.Wallet;

using Base.Requests;
using Base.Responses;
using Domain.Entities;
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

            var transaction = round.Transactions.FirstOrDefault(t => t.Id == request.TransactionId);
            
            if(transaction is null)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.TransactionDoesNotExist);
            
            if (round.Finished)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.Unknown);
            
            if (round.User.Currency.Name != request.Currency)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);

            round.User.Balance += request.Amount;

            round.Transactions.Remove(transaction);
            
            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new BalanceResponse(round.User.Balance);

            return ResultFactory.Success(response);
        }
    }
}