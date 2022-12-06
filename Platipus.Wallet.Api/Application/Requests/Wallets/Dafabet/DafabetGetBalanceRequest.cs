namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DafabetGetBalanceRequest(
    string PlayerId,
    string Hash) : IDafabetBaseRequest, IRequest<IDafabetResult<DafabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DafabetGetBalanceRequest, IDafabetResult<DafabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDafabetResult<DafabetBalanceResponse>> Handle(
            DafabetGetBalanceRequest request,
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
                return DafabetResultFactory.Failure<DafabetBalanceResponse>(DafabetErrorCode.PlayerNotFound);

            var response = new DafabetBalanceResponse(user.UserName, user.CurrencyName, user.Balance);

            return DafabetResultFactory.Success(response);
        }
    }

    public string GetSource()
        => PlayerId;
}