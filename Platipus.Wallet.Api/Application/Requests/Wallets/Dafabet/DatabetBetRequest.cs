namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Dafabet;
using Results.Dafabet.WithData;

public record DatabetBetRequest(
    string PlayerId,
    decimal Amount,
    string GameCode,
    string RoundId,
    string TransactionId,
    string Device,
    string Hash) : DatabetBaseRequest(PlayerId, Hash), IRequest<IDafabetResult<DatabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DatabetBetRequest, IDafabetResult<DatabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDafabetResult<DatabetBalanceResponse>> Handle(
            DatabetBetRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId && r.User.UserName == request.PlayerId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                var user = await _context.Set<User>()
                    .Where(u => u.UserName == request.PlayerId)
                    .Include(u => u.Currency)
                    .FirstAsync(cancellationToken);

                round = new Round
                {
                    Id = request.RoundId,
                    Finished = false,
                    User = user
                };
                _context.Add(round);

                await _context.SaveChangesAsync(cancellationToken);
            }

            if (round.Transactions.Any(t => t.Id == request.TransactionId))
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.TransactionNotFound);

            if (round.Finished)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.TransactionNotFound);

            round.User.Balance -= request.Amount;

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

    public override string GetSource()
    {
        return PlayerId + Amount + GameCode + RoundId + TransactionId + Device;
    }
}