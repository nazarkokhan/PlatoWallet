namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DafabetCancelBetRequest(
    string PlayerId,
    decimal Amount,
    string GameCode,
    string RoundId,
    string OriginalTransactionId,
    string Hash) : IDafabetBaseRequest, IRequest<IDafabetResult<DafabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DafabetCancelBetRequest, IDafabetResult<DafabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDafabetResult<DafabetBalanceResponse>> Handle(
            DafabetCancelBetRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId && r.User.UserName == request.PlayerId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null || round.Finished)
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.RoundNotFound);

            var lastTransaction = round.Transactions.MaxBy(t => t.CreatedDate);
            if (lastTransaction is null || lastTransaction.Id != request.OriginalTransactionId)
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.TransactionNotFound);

            var user = round.User;

            user.Balance += request.Amount;
            _context.Update(user);

            round.Transactions.Remove(lastTransaction);
            _context.Update(round);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new DafabetBalanceResponse(user.UserName, user.Currency.Name, user.Balance);

            return DafabetResultFactory.Success(response);
        }
    }

    public string GetSource()
        => PlayerId + Amount + GameCode + RoundId + OriginalTransactionId;
}