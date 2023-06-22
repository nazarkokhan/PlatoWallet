namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Base;
using Responses;
using Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Domain.Entities;
using Infrastructure.Persistence;

public sealed record EmaraPlayRefundRequest(
    string User, 
    string Transaction, 
    [property: JsonPropertyName("originalTransaction")]string OriginalTransaction, 
    decimal Amount, 
    [property: JsonPropertyName("bonusAmount")]string BonusAmount, 
    string Provider, 
    string Bet, 
    string Game, 
    string Token) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayRefundResponse>>
{
    public sealed class Handler : 
        IRequestHandler<EmaraPlayRefundRequest, IEmaraPlayResult<EmaraPlayRefundResponse>>
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly ILogger<Handler> _logger;

        public Handler(WalletDbContext walletDbContext, ILogger<Handler> logger)
        {
            _walletDbContext = walletDbContext;
            _logger = logger;
        }

        public async Task<IEmaraPlayResult<EmaraPlayRefundResponse>> Handle(
            EmaraPlayRefundRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var dbTransaction = await _walletDbContext.Database.BeginTransactionAsync(cancellationToken);

                var user = await _walletDbContext.Set<User>()
                    .TagWith("Refund")
                    .Where(u => u.Username == request.User &&
                            u.Sessions.Any(s => s.Id == request.Token))
                    .Include(
                        u => u.Rounds.Where(
                            r => r.Id == request.Bet && 
                                 r.Transactions.Any(t => t.Id == request.Transaction && 
                                                         t.InternalId == request.OriginalTransaction))
                        )
                    .ThenInclude(r => r.Transactions.Where(t => t.Id == request.Transaction))
                    .Include(g => g.Casino.CasinoGames)
                    .ThenInclude(cg => cg.Game)
                    .Where(g => g.Casino.CasinoGames.Any(cg => 
                        cg.Game.Id.ToString() == request.Game))
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(cancellationToken);

                if (user is null)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.PlayerNotFound);

                if (user.IsDisabled)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.GameIsNotFoundOrDisabled);

                var round = user.Rounds.FirstOrDefault();
                if (round is null)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.RoundNotFound);

                if (round.Finished)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.BetRoundAlreadyClosed);

                var transaction = round.Transactions.FirstOrDefault();
                if (transaction is null || transaction.IsCanceled)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.TransactionNotFound);

                transaction.Cancel();
                _walletDbContext.Update(transaction);

                user.Balance += transaction.Amount;
                _walletDbContext.Update(user);

                await _walletDbContext.SaveChangesAsync(cancellationToken);

                await dbTransaction.CommitAsync(cancellationToken);

                var refundResult = new RefundResult(user.Currency.Id, user.Balance, transaction.Id, transaction.InternalId);
                var response = new EmaraPlayRefundResponse(refundResult);

                return EmaraPlayResultFactory.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception during refund operation has occurred");
                return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.InternalServerError, e);
            }
        }
    }
}