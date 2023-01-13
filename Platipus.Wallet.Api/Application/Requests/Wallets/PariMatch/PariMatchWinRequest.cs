// ReSharper disable IdentifierTypo

// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.PariMatch;
using Results.PariMatch.WithData;
using static Results.PariMatch.PariMatchResultFactory;
public record PariMatchWinRequest(
    string Cid,
    string PlayerId,
    string Productid,
    string TxId,
    string RoundId,
    bool RoundClosed,
    int Amount,
    string Currency) : IRequest<IPariMatchResult<PariMatchWinRequest.PariMatchWinResponse>>
{
    public record PariMatchWinResponse(
        DateTime CreatedAt,
        int Balance,
        string Txid,
        string ProcessedTxId,
        bool AlreadyProcessed);
    
    public class Handler : IRequestHandler<PariMatchWinRequest, IPariMatchResult<PariMatchWinResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPariMatchResult<PariMatchWinResponse>> Handle(
            PariMatchWinRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                return Failure<PariMatchWinResponse>(
                    PariMatchErrorCode.IntegrationHubFailure,
                    new Exception("Round isn't found"));
            }

            if (round.Transactions.Any(t => t.Id == request.TxId))
                return Failure<PariMatchWinResponse>(PariMatchErrorCode.IntegrationHubFailure, new Exception("Double transaction"));

            if (round.Finished)
                return Failure<PariMatchWinResponse>(PariMatchErrorCode.IntegrationHubFailure, new Exception("Round is finished"));

            var user = round.User;

            user.Balance += request.Amount;

            var transaction = new Transaction
            {
                Id = request.TxId,
                Amount = request.Amount
            };

            round.Transactions.Add(transaction);

            _context.Update(user);
            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new PariMatchWinResponse(
                transaction.CreatedDate,
                (int)user.Balance,
                transaction.Id,
                "Processed TxId",
                false);

            return Success(response);
        }
    }
}