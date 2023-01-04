namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Results.Everymatrix;
using Results.Everymatrix.WithData;

public record EveryMatrixReconciliationRequest(
    DateTime FromDate,
    DateTime ToDate,
    string UserId) : IRequest<IEverymatrixResult<EveryMatrixReconciliationRequest.Response>>
{
    public class Handler
        : IRequestHandler<EveryMatrixReconciliationRequest, IEverymatrixResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEverymatrixResult<Response>> Handle(
            EveryMatrixReconciliationRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>().Where(u => u.Id == new Guid(request.UserId)).FirstOrDefaultAsync();

            var currency = await _context.Set<Currency>().FirstOrDefaultAsync(c => c.Id == user.CurrencyId);

            if (user is null)
            {
                return EverymatrixResultFactory.Failure<Response>(EverymatrixErrorCode.UnknownError);
            }


            var transaction = await _context.Set<Transaction>()
                .Where(
                    t => t.Round.UserId
                      == new Guid(request.UserId)
                      && t.CreatedDate >= request.FromDate.ToUniversalTime()
                      && t.CreatedDate <= request.ToDate.ToUniversalTime())
                .Select(
                    t => new
                    {
                        UserId = t.Round.UserId,
                        BetAmount = t.Amount,
                        Win = "need to add relationship to DB",
                        RoundId = t.RoundId,
                        ExternalId = t.Id,
                        GameId = "Nedd to add relationship to Db",
                        @DateTime = t.CreatedDate
                    })
                .ToArrayAsync();
            if (transaction.Length == 0)
            {
                return EverymatrixResultFactory.Failure<Response>(EverymatrixErrorCode.TransactionNotFound);
            }

            var response = new Response(
                TotalAmount: (transaction.Length + 1),
                Transactions: transaction,
                Currency: currency.Name,
                ErrorData: default,
                ErrorCode: default);


            return EverymatrixResultFactory.Success(response);
        }
    }

    public record Response(
        int TotalAmount,
        Array Transactions,
        string Currency,
        string ErrorData,
        int ErrorCode);
}