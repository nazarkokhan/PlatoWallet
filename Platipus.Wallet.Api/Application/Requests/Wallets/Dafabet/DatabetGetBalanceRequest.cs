namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DatabetGetBalanceRequest(
    string PlayerId,
    string Hash) : IDatabetBaseRequest, IRequest<IDafabetResult<DatabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DatabetGetBalanceRequest, IDafabetResult<DatabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDafabetResult<DatabetBalanceResponse>> Handle(
            DatabetGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.PlayerId)
                .Select(
                    s => new
                    {
                        s.Id,
                        s.UserName,
                        s.Balance,
                        CurrencyName = s.Currency.Name,
                        s.IsDisabled
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null || user.IsDisabled)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DafabetErrorCode.PlayerNotFound);

            var response = new DatabetBalanceResponse(user.UserName, user.CurrencyName, user.Balance);

            return DatabetResultFactory.Success(response);
        }
    }

    public string GetSource()
        => PlayerId;
}