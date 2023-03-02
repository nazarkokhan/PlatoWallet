namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DafabetAuthorizeRequest(
    string PlayerId,
    string PlayerToken,
    string Hash) : IDafabetRequest, IRequest<IDafabetResult<DafabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DafabetAuthorizeRequest, IDafabetResult<DafabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDafabetResult<DafabetBalanceResponse>> Handle(
            DafabetAuthorizeRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Username == request.PlayerId)
                .Select(
                    u => new
                    {
                        u.Id,
                        u.Username,
                        u.Balance,
                        Currency = u.Currency.Id,
                        u.IsDisabled,
                        Session = u.Sessions
                            .Where(s => s.Id == request.PlayerToken)
                            .Select(
                                s => new
                                {
                                    s.Id,
                                    s.ExpirationDate
                                })
                            .FirstOrDefault()
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null || user.IsDisabled)
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.PlayerNotFound);

            if (user.Session is null || user.Session.ExpirationDate < DateTime.UtcNow)
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.InvalidToken);

            var response = new DafabetBalanceResponse(user.Username, user.Currency, user.Balance);

            return DafabetResultFactory.Success(response);
        }
    }

    public string GetSource()
    {
        return PlayerId + PlayerToken;
    }
}