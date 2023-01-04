namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Microsoft.EntityFrameworkCore;
public record EverymatrixBetRequest(
    string TransactionId,
    string Token,
    string Currency,
    decimal Amount,
    string Hash) :IEveryMatrixBaseRequest, IRequest<IEverymatrixResult<EveryMatrixBaseResponse>>
{
    public class Handler : IRequestHandler<EverymatrixBetRequest, IEverymatrixResult<EveryMatrixBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEverymatrixResult<EveryMatrixBaseResponse>> Handle(
            EverymatrixBetRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .Where(s => s.Id == new Guid(request.Token))
                .FirstAsync(cancellationToken);

            var round = await _context.Set<Round>()
                        .Where(r => r.UserId == session.UserId)
                        .Include(r => r.User.Currency)
                        .Include(r => r.Transactions)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (round is null)
                    {

                        var thisUser = await _context.Set<User>()
                            .Where(u => u.Id == session.UserId)
                            .Include(u => u.Currency)
                            .FirstOrDefaultAsync();

                        round = new Round
                        {
                            Id = Guid.NewGuid().ToString(),
                            Finished = false,
                            User = thisUser
                        };

                        _context.Add(round);

                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    var user = await _context.Set<User>()
                        .Where(u => u.Id == session.UserId)
                        .Include(u => u.Currency)
                        .FirstOrDefaultAsync();


                    if (round.Transactions.Any(t => t.Id == request.TransactionId))
                        return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(
                            EverymatrixErrorCode.DoubleTransaction);


                    if (round.Finished)
                        return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.UnknownError);


                    if (user?.Currency.Name != request.Currency)
                        return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.CurrencyDoesntMatch);

                    user.Balance -= request.Amount;

                    if (user.Balance < 0)
                    {
                        return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(
                            EverymatrixErrorCode.InsufficientFunds);
                    }

                    var transaction = new Transaction
                    {
                        Id = request.TransactionId,
                        Amount = request.Amount,
                    };

                    round.Transactions.Add(transaction);


                    _context.Update(user);
                    _context.Update(round);
                    await _context.SaveChangesAsync(cancellationToken);

                    var response = new EveryMatrixBaseResponse(Status:"200", TotalBalance: user.Balance, Currency: user.Currency.Name);

                    return EverymatrixResultFactory.Success(response);
        }
    }
}