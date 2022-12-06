namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DafabetAuthorizeRequest(
    string PlayerId,
    Guid PlayerToken,
    string Hash) : IDafabetBaseRequest, IRequest<IDafabetResult<DafabetBalanceResponse>>
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
                .Where(u => u.UserName == request.PlayerId)
                .Select(
                    u => new
                    {
                        u.Id,
                        u.UserName,
                        u.Balance,
                        CurrencyName = u.Currency.Name,
                        u.IsDisabled,
                        Sessions = u.Sessions
                            .Where(s => s.ExpirationDate > DateTime.UtcNow)
                            .Select(
                                s => new
                                {
                                    s.Id,
                                    s.ExpirationDate
                                })
                            .ToList()
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null || user.IsDisabled)
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.PlayerNotFound);

            if (user.Sessions.All(s => s.Id != request.PlayerToken))
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.InvalidToken);

            var response = new DafabetBalanceResponse(user.UserName, user.CurrencyName, user.Balance);

            return DafabetResultFactory.Success(response);
        }
    }

    public string GetSource()
    {
        return PlayerId + PlayerToken;
    }
}