namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Openbox;
using Results.Openbox.WithData;

public record OpenboxMoneyTransactionRequest(
        Guid Token,
        string GameUid, //TODO redundant?
        string GameCycleUid,
        string OrderUid,
        int OrderType, //TODO check bet/win
        long OrderAmount,
        OpenboxSingleRequest Request)
    : OpenboxBaseRequest(Token, Request), IRequest<IOpenboxResult<OpenboxBalanceResponse>>
{
    public class Handler : IRequestHandler<OpenboxMoneyTransactionRequest, IOpenboxResult<OpenboxBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<OpenboxBalanceResponse>> Handle(
            OpenboxMoneyTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(
                    r => r.Id == request.GameCycleUid &&
                         r.User.UserName == request.OrderUid)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                var user = await _context.Set<User>()
                    .Where(u => u.UserName == request.OrderUid)
                    .Include(u => u.Currency)
                    .FirstAsync(cancellationToken);

                round = new Round
                {
                    Id = request.GameCycleUid,
                    Finished = false,
                    User = user
                };
                _context.Add(round);

                await _context.SaveChangesAsync(cancellationToken);
            }

            if (round.Transactions.Any(t => t.Id == request.OrderUid))
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.Success);

            if (round.Finished)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.Success);

            round.User.Balance -= request.OrderAmount;

            var transaction = new Transaction
            {
                Id = request.OrderUid,
                Amount = request.OrderAmount,
            };

            round.Transactions.Add(transaction);

            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new OpenboxBalanceResponse(round.User.Balance);

            return OpenboxResultFactory.Success(response);
        }
    }
}