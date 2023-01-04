namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;

public record EverymatrixCancelRequest(
    string Hash,
    string ExternalId,
    string CanceledExternalId,
    string Token) : IRequest<IEverymatrixResult<EveryMatrixBaseResponse>>, IEveryMatrixBaseRequest
{
    public class Handler : IRequestHandler<EverymatrixCancelRequest, IEverymatrixResult<EveryMatrixBaseResponse>>
    {
        private readonly WalletDbContext _dbContext;


        public Handler(WalletDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEverymatrixResult<EveryMatrixBaseResponse>> Handle(
            EverymatrixCancelRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _dbContext.Set<Session>()
                .Where(s => s.Id == new Guid(request.Token))
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
            {
                return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.TokenNotFound);
            }

            var user = await _dbContext.Set<User>().Where(u => u.Id == session.UserId).FirstOrDefaultAsync();

            if (user is null)
            {
                return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.UnknownError);
            }

            if (user.IsDisabled)
            {
                return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.UserIsBlocked);
            }

            var transaction = await _dbContext.Set<Transaction>()
                .Where(t => t.Id == request.ExternalId)
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
            {
                return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.TransactionNotFound);
            }

            var currency = await _dbContext.Set<Currency>().FirstOrDefaultAsync(c => c.Id == user.CurrencyId);

            user.Balance -= transaction.Amount;

            _dbContext.Remove(transaction);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();


            var response = new EveryMatrixBaseResponse("Ok", user.Balance, currency.Name);

            return EverymatrixResultFactory.Success(response);
        }
    }
}