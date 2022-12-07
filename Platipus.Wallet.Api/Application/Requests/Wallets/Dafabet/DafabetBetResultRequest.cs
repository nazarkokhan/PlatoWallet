namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DafabetBetResultRequest(
    string PlayerId,
    decimal Amount,
    string GameCode,
    string RoundId,
    string TransactionId,
    bool EndRound,
    string? Device,
    string Hash) : IDafabetBaseRequest, IRequest<IDafabetResult<DafabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DafabetBetResultRequest, IDafabetResult<DafabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDafabetResult<DafabetBalanceResponse>> Handle(
            DafabetBetResultRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId && r.User.UserName == request.PlayerId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.RoundNotFound);

            if (round.Transactions.Any(t => t.Id == request.TransactionId))
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.TransactionNotFound);

            if (round.Finished)
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.RoundNotFound);

            round.User.Balance += request.Amount;
            if (request.EndRound)
                round.Finished = request.EndRound;

            var transaction = new Transaction
            {
                Id = request.TransactionId,
                Amount = request.Amount,
            };

            round.Transactions.Add(transaction);

            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new DafabetBalanceResponse(round.User.UserName, round.User.Currency.Name, round.User.Balance);

            return DafabetResultFactory.Success(response);
        }
    }

    public string GetSource()
    {
        return PlayerId + Amount + GameCode + RoundId + TransactionId + EndRound.ToString().ToLower();
    }
}