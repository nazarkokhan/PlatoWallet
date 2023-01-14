// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Api.Extensions.SecuritySign;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Betflag;
using Results.Betflag.WithData;
using Microsoft.EntityFrameworkCore;
using static Results.Betflag.BetflagResultFactory;

public record BetflagBetRequest(
    string Key,
    string TransactionId,
    string RoundId,
    double Bet,
    bool FreeSpin,
    double QuotaJP,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBetWinCancelResponse>>, IBetflagBaseRequest
{
    public class Handler : IRequestHandler<BetflagBetRequest, IBetflagResult<BetflagBetWinCancelResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBetWinCancelResponse>> Handle(
            BetflagBetRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Id == new Guid(request.Key))
                .Include(u => u.Currency)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.InvalidParameter, new Exception("User isn't found"));
            }

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                round = new Round
                {
                    Id = Guid.NewGuid().ToString(),
                    Finished = false,
                    User = user
                };

                _context.Add(round);

                await _context.SaveChangesAsync(cancellationToken);
            }

            var session = await _context.Set<Session>()
                .LastOrDefaultAsync(s => s.Id == new Guid( request.Key));

            if (session is null)
            {
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.InvalidToken);
            }

            var transactionIsRetry = round.Transactions.Any(t => t.Id != request.TransactionId);

            if (!transactionIsRetry)
            {
                var transaction = new Transaction
                {
                    Id = request.TransactionId,
                    Amount = (decimal) request.Bet,
                };

                user.Balance -= (decimal) request.Bet;

                if (user.Balance < 0)
                {
                    return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.InsufficientFunds);
                }

                round.Transactions.Add(transaction);

                _context.Update(user);
                _context.Update(round);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var hash = BetflagRequestHash.Compute(session.Id.ToString(), timeStamp);

            var response = new BetflagBetWinCancelResponse(
                (int) BetflagErrorCode.SUCCSESS,
                BetflagErrorCode.SUCCSESS.ToString(),
                (double) user.Balance,
                false,
                user.Currency.Name,
                "IdTicket",
                session.Id.ToString(),
                transactionIsRetry,
                timeStamp,
                hash);

            return Success(response);
        }
    }
}