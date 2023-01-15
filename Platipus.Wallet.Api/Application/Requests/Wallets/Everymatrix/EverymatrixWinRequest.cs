namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;

public record EverymatrixWinRequest(
    string Hash,
    decimal Amount,
    string Currency,
    string GameId,
    string RoundId,
    string ExternalId,
    string BetExternalId,
    string Token,
    bool RoundEnd,
    string JackpotPayout,
    string JackpotId,
    decimal JackpotPayoutAmount,
    string BonusId
) :IRequest<IEverymatrixResult<EveryMatrixBaseResponse>>,IEveryMatrixBaseRequest
{
    public class Handler : IRequestHandler<EverymatrixWinRequest, IEverymatrixResult<EveryMatrixBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEverymatrixResult<EveryMatrixBaseResponse>> Handle(
            EverymatrixWinRequest request,
            CancellationToken cancellationToken)
        {

            var session = await _context.Set<Session>()
                .Where(s => s.Id == new Guid(request.Token))
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
            {
                return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.TokenNotFound);
            }

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId && r.UserId == session.UserId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null || round.Transactions.Any(t => t.Id == request.BetExternalId))
                return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.DoubleTransaction);

            if (round.Finished)
                return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.UnknownError);

            var user = round.User;
            if (user.Currency.Name != request.Currency)
                return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.CurrencyDoesntMatch);

            user.Balance += request.Amount;

            var transaction = new Transaction
            {
                Id = request.BetExternalId,
                Amount = request.Amount
            };

            round.Transactions.Add(transaction);

            _context.Update(user);
            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new EveryMatrixBaseResponse(
                "Ok",
                user.Balance,
                user.Currency.Name);

            return EverymatrixResultFactory.Success(response);
        }
    }
}
