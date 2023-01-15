namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Api.Extensions.SecuritySign;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.BetConstruct.WithData;
using static Results.BetConstruct.BetConstructResultFactory;
using Microsoft.EntityFrameworkCore;
using Results.BetConstruct;

public record BetConstructDepositRequest(
    DateTime Time,
    object Data,
    string Hash,
    string Token,
    string TransactionId,
    string RoundId,
    string GameId,
    string CurrencyId,
    decimal BetAmount,
    decimal BetInfo) : IRequest<IBetConstructResult<BetConstructBaseResponse>>, IBetConstructBaseRequest
{
    public class Handler : IRequestHandler<BetConstructDepositRequest, IBetConstructResult<BetConstructBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }


        public async Task<IBetConstructResult<BetConstructBaseResponse>> Handle(
            BetConstructDepositRequest request,
            CancellationToken cancellationToken)
        {
            //TODO check if the BetAmount is same to win transaction amount

            var session = await _context.Set<Session>()
                .FirstOrDefaultAsync(s => s.Id == new Guid(request.Token));

            if (session is null)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.TokenNotFound);
            }

            if (session.ExpirationDate < DateTime.UtcNow)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.TokenExpired);
            }

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                return Failure<BetConstructBaseResponse>(
                    BetConstructErrorCode.IncorrectParametersPassed,
                    new Exception("Round isn't found"));
            }

            if (round.Transactions.Any(t => t.Id == request.TransactionId))
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.TransactionIsAlreadyExist);

            if (round.Finished)
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.GameIsBlocked, new Exception("Round is finished"));

            var user = round.User;

            if (user.Currency.Name != request.CurrencyId)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.WrongCurrency);
            }

            if (user.IsDisabled)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.PlayerIsBlocked);
            }

            user.Balance -= request.BetAmount;

            //TODO check if other requests have same check

            if (user.Balance < 0)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.NotEnoughMoney);
            }

            var transaction = new Transaction
            {
                Id = request.TransactionId,
                Amount = request.BetAmount
            };

            round.Transactions.Add(transaction);

            _context.Update(user);
            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new BetConstructBaseResponse(
                true,
                null,
                null,
                transaction.Id,
                user.Balance);

            return Success(response);
        }
    }
}