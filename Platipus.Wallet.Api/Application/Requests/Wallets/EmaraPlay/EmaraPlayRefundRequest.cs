﻿using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayRefundRequest(
    string User, string Transaction, 
    string OriginalTransaction, string Amount,
    string BonusAmount, string Provider, string Bet, 
    string Game, string Token) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayRefundResponse>>
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
                    .ToListAsync(cancellationToken);
                var matchedUser = user.FirstOrDefault(u => 
                    u.Casino.Provider.ToString() == request.Provider);
                
                if (matchedUser is null)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.PlayerNotFound);

                if (matchedUser.IsDisabled)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.GameIsNotFoundOrDisabled);

                var round = matchedUser.Rounds.FirstOrDefault();
                if (round is null)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.RoundNotFound);

                if (round.Finished)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.BetRoundAlreadyClosed);

                var transaction = round.Transactions.FirstOrDefault();
                if (transaction is null || transaction.IsCanceled)
                    return EmaraPlayResultFactory.Failure<EmaraPlayRefundResponse>(EmaraPlayErrorCode.TransactionNotFound);

                transaction.Cancel();
                _walletDbContext.Update(transaction);

                matchedUser.Balance += transaction.Amount;
                _walletDbContext.Update(user);

                await _walletDbContext.SaveChangesAsync(cancellationToken);

                await dbTransaction.CommitAsync(cancellationToken);

                var refundResult = new RefundResult(matchedUser.Currency.Id, matchedUser.Balance.ToString(CultureInfo.InvariantCulture), 
                    transaction.Id, transaction.InternalId);
                var response = new EmaraPlayRefundResponse(((int)EmaraPlayErrorCode.Success).ToString(), 
                    EmaraPlayErrorCode.Success.ToString(), refundResult);

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