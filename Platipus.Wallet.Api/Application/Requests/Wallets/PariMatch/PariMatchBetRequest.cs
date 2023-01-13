// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch;

using Domain.Entities;
using Infrastructure.Persistence;
using Results.PariMatch;
using Results.PariMatch.WithData;
using static Results.PariMatch.PariMatchResultFactory;
using Microsoft.EntityFrameworkCore;

public record PariMatchBetRequest(
    string Cid,
    string SessionToken,
    string PlayerId,
    string ProductId,
    string TxId,
    string RoundId,
    bool RoundClosed,
    int Amount,
    string Currency) : IRequest<IPariMatchResult<PariMatchBetRequest.PariMatchBetResponse>>
{
    public record PariMatchBetResponse(
        string TxId,
        string ProcessedTxId,
        DateTime CreatedAt,
        int Balance,
        bool AlreadyProcessed);

    public class Handler : IRequestHandler<PariMatchBetRequest, IPariMatchResult<PariMatchBetResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPariMatchResult<PariMatchBetResponse>> Handle(
            PariMatchBetRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .FirstOrDefaultAsync(s => s.Id == new Guid(request.SessionToken));

            if (session is null)
            {
                return Failure<PariMatchBetResponse>(PariMatchErrorCode.InvalidSessionKey);
            }

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                var thisUser = await _context.Set<User>()
                    .Where(u => u.Id == session.UserId)
                    .Include(u => u.Currency)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                round = new Round
                {
                    Id = Guid.NewGuid().ToString(),
                    Finished = false,
                    User = thisUser
                };

                _context.Add(round);

                await _context.SaveChangesAsync(cancellationToken);
            }

            var user = await _context.Set<User>()
                .Where(u => u.Id == new Guid(request.PlayerId))
                .Include(u => u.Currency)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return Failure<PariMatchBetResponse>(PariMatchErrorCode.IntegrationHubFailure, new Exception("User isn't found"));
            }

            if (round.Transactions.Any(t => t.Id == request.TxId))
                return Failure<PariMatchBetResponse>(PariMatchErrorCode.InvalidTransactionId);

            user.Balance -= request.Amount;

            if (user.Balance < 0)
            {
                return Failure<PariMatchBetResponse>(PariMatchErrorCode.InsufficientBalance);
            }

            var transaction = new Transaction
            {
                Id = request.TxId,
                Amount = request.Amount,
            };

            round.Transactions.Add(transaction);

            _context.Update(user);
            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new PariMatchBetResponse(
                request.TxId,
                "TxId on Eva",
                transaction.CreatedDate,
                (int) user.Balance,
                false);

            return Success(response);
        }
    }
}