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

public record BetflagCancelRequest(
    string Key,
    string TransactionId,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBetWinCancelResponse>>, IBetflagBaseRequest
{
    public class Handler : IRequestHandler<BetflagCancelRequest, IBetflagResult<BetflagBetWinCancelResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBetWinCancelResponse>> Handle(
            BetflagCancelRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Id == new Guid(request.Key), cancellationToken: cancellationToken);

            if (user is null)
            {
                return Failure<BetflagBetWinCancelResponse>(
                    BetflagErrorCode.InvalidParameter,
                    new Exception("User isn't found"));
            }

            if (user.IsDisabled)
            {
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.Exception, new Exception("User is Blocked"));
            }

            var session = await _context.Set<Session>()
                .LastOrDefaultAsync(s => s.Id == new Guid(request.Key));

            if (session is null)
            {
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.InvalidToken);
            }

            var round = await _context.Set<Round>()
                .Where(r => r.Transactions.Any(t => t.Id == request.TransactionId) && r.UserId == user.Id)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
                return Failure<BetflagBetWinCancelResponse>(
                    BetflagErrorCode.RoundEndBetNotExists,
                    new Exception("Round isn't found"));

            if (round.Finished)
                return Failure<BetflagBetWinCancelResponse>(
                    BetflagErrorCode.RoundEndBetNotExists,
                    new Exception("Round is finished"));

            var transaction = await _context.Set<Transaction>()
                .Where(t => t.Id == request.TransactionId)
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
            {
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.InvalidParameter);
            }

            user.Balance -= transaction.Amount;

            _context.Remove(transaction);
            _context.Update(user);
            await _context.SaveChangesAsync();

            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var hash = BetflagRequestHash.Compute("0", timeStamp).ToUpperInvariant();

            var response = new BetflagBetWinCancelResponse(
                (int) BetflagErrorCode.SUCCSESS,
                BetflagErrorCode.SUCCSESS.ToString(),
                (double) user.Balance,
                false,
                user.Currency.Name,
                "IdTicket",
                session.Id.ToString(),
                false,
                timeStamp,
                hash);

            return Success(response);
        }
    }
}