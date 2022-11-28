namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DatabetBetResultRequest(
    string PlayerId,
    decimal Amount,
    string GameCode,
    string RoundId,
    string TransactionId,
    bool EndRound,
    string? Device,
    string Hash) : IDatabetBaseRequest, IRequest<IDafabetResult<DatabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DatabetBetResultRequest, IDafabetResult<DatabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDafabetResult<DatabetBalanceResponse>> Handle(
            DatabetBetResultRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId && r.User.UserName == request.PlayerId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.RoundNotFound);

            if (round.Transactions.Any(t => t.Id == request.TransactionId))
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.TransactionNotFound);

            if (round.Finished)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.RoundNotFound);

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

            var response = new DatabetBalanceResponse(round.User.UserName, round.User.Currency.Name, round.User.Balance);

            return DatabetResultFactory.Success(response);
        }
    }

    public string GetSource()
    {
        return PlayerId + Amount + GameCode + RoundId + TransactionId + EndRound.ToString().ToLower();
    }
}