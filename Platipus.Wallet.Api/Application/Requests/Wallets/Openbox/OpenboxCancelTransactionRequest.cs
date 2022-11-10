namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Openbox;
using Results.Openbox.WithData;

public record OpenboxCancelTransactionRequest(
    string Token,
    string GameUid,
    string GameCycleUid,
    string OrderUid,
    string OrderUidCancel) : OpenboxBaseRequest(Token), IRequest<IOpenboxResult<OpenboxBalanceResponse>>
{
    public class Handler : IRequestHandler<OpenboxCancelTransactionRequest, IOpenboxResult<OpenboxBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<OpenboxBalanceResponse>> Handle(
            OpenboxCancelTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.GameCycleUid && r.User.Sessions.Any(s => s.Id == new Guid(request.Token)))
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.ParameterError);

            if (round.Finished)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.ParameterError);

            var lastTransaction = round.Transactions.MaxBy(t => t.CreatedDate);
            if (lastTransaction is null || lastTransaction.Id != request.OrderUid)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.ParameterError);

            var user = round.User;

            user.Balance += lastTransaction.Amount / 100;
            _context.Update(user);

            round.Transactions.Remove(lastTransaction);
            _context.Update(round);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new OpenboxBalanceResponse((long) (user.Balance * 100));

            return OpenboxResultFactory.Success(response);
        }
    }
}