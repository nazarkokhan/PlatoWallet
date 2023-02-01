namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.BetConstruct;
using Results.BetConstruct.WithData;
using static Results.BetConstruct.BetConstructResultFactory;

public record BetConstructRollbackTransactionRequest(
    DateTime Time,
    string Hash,
    object Data,
    string Token,
    string TransactionId,
    string GameId) : IRequest<IBetConstructResult<BetConstructBaseResponse>>, IBetConstructBaseRequest
{
    public class Handler : IRequestHandler<BetConstructRollbackTransactionRequest, IBetConstructResult<BetConstructBaseResponse>>
    {
        private readonly WalletDbContext _context;


        public Handler(WalletDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<IBetConstructResult<BetConstructBaseResponse>> Handle(
            BetConstructRollbackTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .FirstOrDefaultAsync(s => s.Id == new Guid(request.Token));

            if (session is null)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.TokenNotFound);
            }

            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Id == session.UserId, cancellationToken: cancellationToken);

            if (user is null)
            {
                return Failure<BetConstructBaseResponse>(
                    BetConstructErrorCode.IncorrectParametersPassed,
                    new Exception("User isn;t found"));
            }

            if (user.IsDisabled)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.GameIsBlocked);
            }

            var round = await _context.Set<Round>()
                .Where(r => r.UserId == session.UserId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
                return Failure<BetConstructBaseResponse>(
                    BetConstructErrorCode.IncorrectParametersPassed,
                    new Exception("Round isn't found"));

            if (round.Finished)
                return Failure<BetConstructBaseResponse>(
                    BetConstructErrorCode.IncorrectParametersPassed,
                    new Exception("Round is finished"));

            var transaction = await _context.Set<Transaction>()
                .Where(t => t.Id == request.TransactionId)
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.TransactionNotFound);
            }

            user.Balance -= transaction.Amount;

            _context.Remove(transaction);
            _context.Update(user);
            await _context.SaveChangesAsync();

            var response = new BetConstructBaseResponse(
                true,
                null,
                null,
                transaction.Id,
                user.Balance);

            return Success(response);
        }
    }
}