// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Globalization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.EmaraPlay.WithData;
using Microsoft.EntityFrameworkCore;
using Results.EmaraPlay;
using static Results.EmaraPlay.EmaraPlayResultFactory;

public record EmaraPlayBetRequest(
    string User,
    string Game,
    string Bet,
    string Provider,
    string Token,
    string Transaction,
    string Amount) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBaseResponse>>
{
    public class Handler : IRequestHandler<EmaraPlayBetRequest, IEmaraPlayResult<EmaraPlayBaseResponse>>
    {
        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        private readonly WalletDbContext _context;

        public async Task<IEmaraPlayResult<EmaraPlayBaseResponse>> Handle(
            EmaraPlayBetRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .FirstOrDefaultAsync(s => s.Id == new Guid(request.Token), cancellationToken: cancellationToken);

            if (session is null)
            {
                return Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerAuthenticationFailed);
            }

            var user = await _context.Set<User>()
                .Where(u => u.Id == new Guid(request.User))
                .Include(u => u.Currency)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user is null)
            {
                return Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerNotFound);
            }

            // var isGameValid = await _context.Set<CasinoGames>()
            //     .Where(cg => cg.CasinoId == user.CasinoId && cg.GameId == Int32.Parse(request.Game)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            //
            // if (isGameValid is null)
            // {
            //     return Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.GameIsNotFoundOrDisabled);
            // }

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.Bet)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                var thisUser = await _context.Set<User>()
                    .Where(u => u.Id == session.UserId)
                    .Include(u => u.Currency)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                round = new Round
                {
                    Id = Guid.NewGuid().ToString(),
                    Finished = false,
                    User = thisUser
                };

                _context.Add(round);

                await _context.SaveChangesAsync(cancellationToken);
            }

            if (round.Transactions.Any(t => t.Id == request.Transaction))
                return Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.TransactionNotFound);

            user.Balance -= Convert.ToDecimal(request.Amount);

            if (user.Balance < 0)
            {
                return Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.InsufficientBalance);
            }

            var transaction = new Transaction
            {
                Id = request.Transaction,
                Amount = Convert.ToDecimal(request.Amount),
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

            return Success(response);
        }
    }
}