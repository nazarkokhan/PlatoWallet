namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;

public record EverymatrixReconciliationRequest(
    DateTime FromDate,
    DateTime ToDate,
    string UserId) : IRequest<IEverymatrixResult<EverymatrixReconciliationRequest.ReconciliationResponse>>, IEveryMatrixBaseRequest
{
    public class Handler
        : IRequestHandler<EverymatrixReconciliationRequest, IEverymatrixResult<ReconciliationResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEverymatrixResult<ReconciliationResponse>> Handle(
            EverymatrixReconciliationRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Id == new Guid(request.UserId))
                .Select(u => new { Currency = u.Currency.Name })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return EverymatrixResultFactory.Failure<ReconciliationResponse>(EverymatrixErrorCode.UserIsBlocked);

            var transactionQuery = _context.Set<Transaction>()
                .Where(
                    t => t.Round.UserId
                      == new Guid(request.UserId)
                      && t.CreatedDate >= request.FromDate.ToUniversalTime()
                      && t.CreatedDate <= request.ToDate.ToUniversalTime());

            var transactionCount = await transactionQuery.CountAsync(cancellationToken);

            var transaction = await transactionQuery
                .Select(
                    t => new
                    {
                        UserId = t.Round.UserId,
                        BetAmount = t.Amount,
                        Win = "",
                        RoundId = t.RoundId,
                        ExternalId = t.Id,
                        GameId = "",
                        DateTime = t.CreatedDate
                    })
                .ToListAsync(cancellationToken);

            var response = new ReconciliationResponse(
                transactionCount,
                transaction.ToArray(),
                user.Currency,
                "",
                0);

            return EverymatrixResultFactory.Success(response);
        }
    }

    public record ReconciliationResponse(
        int TotalAmount,
        Array Transactions,
        string Currency,
        string ErrorData,
        int ErrorCode);
}