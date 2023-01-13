// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Globalization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.EmaraPlay;
using Results.EmaraPlay.WithData;

public record EmaraPlayResultRequest(
    string User,
    string Game,
    string Bet,
    string Amount,
    string Transaction,
    string Provider,
    string Token,
    string CloseRound,
    string Jackpots,
    string BonusCode,
    string BonusAmount,
    string Details,
    string BetBonusAmount,
    string Spins) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBaseResponse>>
{
    public class Handler : IRequestHandler<EmaraPlayResultRequest, IEmaraPlayResult<EmaraPlayBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEmaraPlayResult<EmaraPlayBaseResponse>> Handle(
            EmaraPlayResultRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .Where(s => s.Id == new Guid(request.Token))
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
            {
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerAuthenticationFailed);
            }

            var user = await _context.Set<User>()
                .Where(u => u.Id == new Guid(request.User))
                .Include(u => u.Currency)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user is null)
            {
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerNotFound);
            }

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.Bet)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null || round.Transactions.Any(t => t.Id == request.Bet))
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.DuplicatedTransaction);

            if (round.Finished)
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.BetRoundAlreadyClosed);


            user.Balance += Convert.ToDecimal( request.Amount);

            var transaction = new Transaction
            {
                Id = request.Bet,
                Amount = Convert.ToDecimal( request.Amount)
            };

            round.Transactions.Add(transaction);

            _context.Update(user);
            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

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