// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Api.Extensions.SecuritySign;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Betflag;
using Results.Betflag.WithData;
using static Results.Betflag.BetflagResultFactory;
public record BetflagWinRequest(
    string Key,
    string TransactionId,
    string RoundId,
    bool RoundCompleted,
    double Win,
    double WinJp,
    long Timestamp,
    string Hash,
    string ApiName): IRequest<IBetflagResult<BetflagBetWinCancelResponse>>, IBetflagBaseRequest
{
    public class Handler : IRequestHandler<BetflagWinRequest, IBetflagResult<BetflagBetWinCancelResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBetWinCancelResponse>> Handle(
            BetflagWinRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .LastOrDefaultAsync(s => s.Id == new Guid( request.Key));

            if (session is null)
            {
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.InvalidToken);
            }

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                return Failure<BetflagBetWinCancelResponse>(
                    BetflagErrorCode.RoundEndBetNotExists,
                    new Exception("Round isn't found"));
            }

            if (round.Transactions.Any(t => t.Id == request.TransactionId))
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.InvalidParameter, new Exception("Double transaction"));

            if (round.Finished)
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.InvalidParameter, new Exception("Round is finished"));

            var user = round.User;


            var transactionIsRetry = round.Transactions.Any(t => t.Id != request.TransactionId);

            if (!transactionIsRetry)
            {
                var transaction = new Transaction
                {
                    Id = request.TransactionId,
                    Amount = (decimal) request.Win,
                };

                user.Balance += (decimal) request.Win;

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