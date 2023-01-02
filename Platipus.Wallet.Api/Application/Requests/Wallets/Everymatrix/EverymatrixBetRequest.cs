namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Microsoft.EntityFrameworkCore;
public record EverymatrixBetRequest(
    string TransactionId,
    Guid Token,
    string RoundId,
    string GameId,
    string Currency,
    decimal Amount) : IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>
{
    public class Handler : IRequestHandler<EverymatrixBetRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixBetRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                        .Where(r => r.Id == request.RoundId)
                        .Include(r => r.User.Currency)
                        .Include(r => r.Transactions)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (round is null)
                    {
                        var session = await _context.Set<Session>()
                            .Where(s => s.Id == request.Token)
                            .FirstAsync(cancellationToken);

                        var thisUser = await _context.Set<User>()
                            .Where(u => u.Id == session.UserId)
                            .Include(u => u.Currency)
                            .FirstOrDefaultAsync();

                        round = new Round
                        {
                            Id = request.RoundId,
                            Finished = false,
                            User = thisUser
                        };
                        _context.Add(round);

                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    var user = round.User;

                    if (round.Transactions.Any(t => t.Id == request.TransactionId))
                        return EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(
                            EverymatrixErrorCode.DoubleTransaction);


                    if (round.Finished)
                        return EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.UnknownError);


                    if (user?.Currency.Name != request.Currency)
                        return EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.CurrencyDoesntMatch);

                    round.User.Balance -= request.Amount;

                    var transaction = new Transaction
                    {
                        Id = request.TransactionId,
                        Amount = request.Amount,
                    };

                    round.Transactions.Add(transaction);

                    _context.Update(round);
                    await _context.SaveChangesAsync(cancellationToken);

                    var response = new EverymatrixBalanceResponse(Status:"200", TotalBalance: user.Balance, Currency: user.Currency.Name);

                    return EverymatrixResultFactory.Success(response);
        }
    }
}