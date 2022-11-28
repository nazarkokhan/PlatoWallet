namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record PswGetBalanceRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game) : IPswBaseRequest, IRequest<IPswResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<PswGetBalanceRequest, IPswResult<PswBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<PswBalanceResponse>> Handle(
            PswGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .TagWith("GetUserBalance")
                .Where(
                    u => u.UserName == request.User
                      && u.Sessions
                             .Select(s => s.Id)
                             .Contains(request.SessionId))
                .Select(
                    s => new
                    {
                        s.Id,
                        s.Balance,
                        s.IsDisabled
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.InvalidUser);

            if (user.IsDisabled)
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.UserDisabled);

            var response = new PswBalanceResponse(user.Balance);

            return PswResultFactory.Success(response);
        }
    }
}