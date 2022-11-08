namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Dafabet;
using Results.Dafabet.WithData;

public record DatabetCancelBetRequest(
    string PlayerId,
    decimal Amount,
    string GameCode,
    string RoundId,
    string OriginalTransactionId,
    string Hash) : DatabetBaseRequest(PlayerId, Hash), IRequest<IDafabetResult<DatabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DatabetCancelBetRequest, IDafabetResult<DatabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDafabetResult<DatabetBalanceResponse>> Handle(
            DatabetCancelBetRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(
                    r => r.Id == request.RoundId &&
                         r.User.UserName == request.PlayerId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null || round.Finished)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.RoundNotFound);

            var lastTransaction = round.Transactions.MaxBy(t => t.CreatedDate);
            if (lastTransaction is null || lastTransaction.Id != request.OriginalTransactionId)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.TransactionNotFound);

            var user = round.User;

            user.Balance += request.Amount;
            _context.Update(user);

            round.Transactions.Remove(lastTransaction);
            _context.Update(round);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new DatabetBalanceResponse(user.UserName, user.Currency.Name, user.Balance);

            return DatabetResultFactory.Success(response);
        }
    }

    public override string GetSource()
    {
        return PlayerId + Amount + GameCode + RoundId + OriginalTransactionId;
    }
}