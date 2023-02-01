namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Betflag;
using Results.Betflag.WithData;

public record BetflagAuthenticateRequest(
    string Key,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBalanceResponse>>, IBetflagRequest
{
    public class Handler : IRequestHandler<BetflagAuthenticateRequest, IBetflagResult<BetflagBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBalanceResponse>> Handle(
            BetflagAuthenticateRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == new Guid(request.Key)))
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
                return BetflagResultFactory.Failure<BetflagBalanceResponse>(BetflagErrorCode.InvalidParameter);

            var response = new BetflagBalanceResponse(
                (double)user.Balance,
                user.Currency,
                user.UserName,
                user.UserName);

            return BetflagResultFactory.Success(response);
        }
    }
}