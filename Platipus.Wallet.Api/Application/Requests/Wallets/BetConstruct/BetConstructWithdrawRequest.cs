// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Api.Extensions.SecuritySign;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.BetConstruct.WithData;
using static Results.BetConstruct.BetConstructResultFactory;
using Results.BetConstruct;

public record BetConstructWithdrawRequest(
    DateTime Time,
    string Hash,
    object Data,
    string Token,
    string TransactionId,
    string RoundId,
    string GameId,
    string CurrencyId,
    decimal BetAmount,
    string BetInfo) : IRequest<IBetConstructResult<BetConstructBaseResponse>>, IBetConstructBaseRequest
{
    public class Handler : IRequestHandler<BetConstructWithdrawRequest, IBetConstructResult<BetConstructBaseResponse>>
    {
        //TODO check if the BetAmount is same to win transaction amount
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IBetConstructResult<BetConstructBaseResponse>> Handle(
            BetConstructWithdrawRequest request,
            CancellationToken cancellationToken)
        {
            var isValidHash = BetConstructVerifyHashExtension.VerifyBetConstructHash(
                request,
                request.Data.ToString(),
                request.Time);

            if (!isValidHash)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.AuthenticationFailed);
            }

            var session = await _context.Set<Session>()
                .FirstOrDefaultAsync(s => s.Id == new Guid(request.Token));

            if (session is null)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.TokenNotFound);
            }


            var user = await _context.Set<User>()
                .Where(u => u.Id == session.UserId)
                .Include(u => u.Currency)
                .FirstOrDefaultAsync();

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.IncorrectParametersPassed, new Exception("User isn't found"));
            }

            if (user.Currency.Name == request.CurrencyId)
            {
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.WrongCurrency);
            }

            if (round is null)
            {

                round = new Round
                {
                    Id = Guid.NewGuid().ToString(),
                    Finished = false,
                    User = user
                };

                _context.Add(round);

                await _context.SaveChangesAsync(cancellationToken);
            }


            if (round.Transactions.Any(t => t.Id == request.TransactionId))
                return Failure<BetConstructBaseResponse>(BetConstructErrorCode.TransactionIsAlreadyExist);

            user.Balance += request.BetAmount;


            var transaction = new Transaction
            {
                Id = request.TransactionId,
                Amount = request.BetAmount,
            };

            round.Transactions.Add(transaction);

            _context.Update(user);
            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new BetConstructBaseResponse(
                true,
                null,
                null,
                Convert.ToInt64(transaction.Id),
                user.Balance);

            return Success(response);
        }
    }
}