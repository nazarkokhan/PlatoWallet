// ReSharper disable UnusedType.Global
// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Globalization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.EmaraPlay;
using Results.EmaraPlay.WithData;

public record EmaraPlayRefundRequest(
    string User,
    string Transaction,
    string OriginalTransaction,
    string Amount,
    string BonusAmount,
    string Provider,
    string Bet,
    string Game,
    string Token) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBaseResponse>>
{
    public class Handler : IRequestHandler<EmaraPlayRefundRequest, IEmaraPlayResult<EmaraPlayBaseResponse>>
    {
        private readonly WalletDbContext _dbContext;


        public Handler(WalletDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEmaraPlayResult<EmaraPlayBaseResponse>> Handle(
            EmaraPlayRefundRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _dbContext.Set<Session>()
                .Where(s => s.Id == new Guid(request.Token))
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
            {
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerAuthenticationFailed);
            }

            var user = await _dbContext.Set<User>()
                .Where(u => u.Id == new Guid(request.User))
                .Include(u => u.Currency)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user is null)
            {
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerNotFound);
            }

            if (user.IsDisabled)
            {
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerIsFrozen);
            }

            var transaction = await _dbContext.Set<Transaction>()
                .Where(t => t.Id == request.Transaction)
                .FirstOrDefaultAsync(cancellationToken);

            var round = await _dbContext.Set<Round>()
                .Where(r => r.Id == request.Bet)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
            {
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.TransactionNotFound);
            }

            if (round is null)
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(
                    EmaraPlayErrorCode.RoundNotFound);

            if (round.Finished)
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(
                    EmaraPlayErrorCode.BetRoundAlreadyClosed);

            var lastTransaction = round.Transactions.MaxBy(t => t.CreatedDate);
            if (lastTransaction is null || lastTransaction.Id != transaction.Id)
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.TransactionNotFound);


            var isAmountValid = Convert.ToDecimal(request.Amount) == transaction.Amount;

            if (!isAmountValid)
            {
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.BadParameters, new Exception("Amount isn[t correct"));
            }

            user.Balance -= transaction.Amount;

            _dbContext.Remove(transaction);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            var response = new EmaraPlayBaseResponse(
                "0",
                EmaraPlayErrorCode.Success.ToString(),
                new Result(
                    Currency: user.Currency.Name,
                    Balance: user.Balance.ToString(CultureInfo.InvariantCulture),
                    Bonus: null,
                    Transaction: transaction.Id,
                    Promo: null,
                    Txid: "Id of the transaction in Emara Play system"));

            return EmaraPlayResultFactory.Success(response);
        }
    }
}