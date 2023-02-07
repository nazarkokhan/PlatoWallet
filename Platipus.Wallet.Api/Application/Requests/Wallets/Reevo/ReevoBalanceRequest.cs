namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Reevo;
using Results.Reevo.WithData;

public record ReevoBalanceRequest(
    string CallerId,
    string CallerPassword,
    string Action,
    int RemoteId,
    string Username,
    string GameIdHash,
    string SessionId,
    Guid GameSessionId,
    string Key) : IRequest<IReevoResult<ReevoSuccessResponse>>, IReevoRequest
{
    public class Handler : IRequestHandler<ReevoBalanceRequest, IReevoResult<ReevoSuccessResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IReevoResult<ReevoSuccessResponse>> Handle(
            ReevoBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == request.GameSessionId))
                .Include(u => u.Currency)
                .Select(
                    u => new
                    {
                        u.Balance,
                        Currency = u.Currency.Name,
                        u.UserName
                    })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user is null)
                return ReevoResultFactory.Failure<ReevoSuccessResponse>(ReevoErrorCode.GeneralError);

            var response = new ReevoSuccessResponse(user.Balance);

            return ReevoResultFactory.Success(response);
        }
    }
}