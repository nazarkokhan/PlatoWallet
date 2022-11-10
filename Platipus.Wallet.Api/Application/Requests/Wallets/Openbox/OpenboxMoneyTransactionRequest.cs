namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Openbox;
using Results.Openbox.WithData;

public record OpenboxMoneyTransactionRequest(
    string Token,
    string GameUid,
    string GameCycleUid,
    string OrderUid,
    int OrderType,
    long OrderAmount) : OpenboxBaseRequest(Token), IRequest<IOpenboxResult<OpenboxBalanceResponse>>
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
            return request.OrderType switch
            {
                3 => await HandleBet(request, cancellationToken),
                4 => await HandleWin(request, cancellationToken),
                _ => OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.ParameterError)
            };
        }

        private async Task<IOpenboxResult<OpenboxBalanceResponse>> HandleBet(
            OpenboxMoneyTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.GameCycleUid && r.User.Sessions.Any(s => s.Id == new Guid(request.Token)))
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
            {
                var user = await _context.Set<User>()
                    .Where(u => u.Sessions.Any(s => s.Id == new Guid(request.Token)))
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

            round.User.Balance -= request.OrderAmount / 100m;

            var transaction = new Transaction
            {
                Id = request.OrderUid,
                Amount = request.OrderAmount,
            };

            round.Transactions.Add(transaction);

            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new OpenboxBalanceResponse(round.User.Balance * 100);

            return OpenboxResultFactory.Success(response);
        }

        private async Task<IOpenboxResult<OpenboxBalanceResponse>> HandleWin(
            OpenboxMoneyTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(
                    r => r.Id == request.GameCycleUid
                      && r.User.Sessions.Any(s => s.Id == new Guid(request.Token)))
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.ParameterError);

            if (round.Transactions.Any(t => t.Id == request.OrderUid))
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.ParameterError);

            if (round.Finished)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.ParameterError);

            round.User.Balance += request.OrderAmount / 100m;

            round.Finished = true; //TODO when is finished?

            var transaction = new Transaction
            {
                Id = request.OrderUid,
                Amount = request.OrderAmount,
            };

            round.Transactions.Add(transaction);

            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new OpenboxBalanceResponse(round.User.Balance * 100);

            return OpenboxResultFactory.Success(response);
        }
    }
}