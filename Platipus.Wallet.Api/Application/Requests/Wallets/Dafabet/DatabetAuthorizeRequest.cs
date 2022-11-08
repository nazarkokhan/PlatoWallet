namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Dafabet;
using Results.Dafabet.WithData;

public record DatabetAuthorizeRequest(
    string PlayerId,
    Guid PlayerToken,
    string Hash) : DatabetBaseRequest(PlayerId, Hash), IRequest<IDafabetResult<DatabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DatabetAuthorizeRequest, IDafabetResult<DatabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDafabetResult<DatabetBalanceResponse>> Handle(
            DatabetAuthorizeRequest request,
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
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.PlayerNotFound);

            if (user.Sessions.All(s => s.Id != request.PlayerToken))
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.InvalidToken);

            var response = new DatabetBalanceResponse(user.UserName, user.CurrencyName, user.Balance);

            return DatabetResultFactory.Success(response);
        }
    }

    public override string GetSource()
    {
        return PlayerId + PlayerToken;
    }
}