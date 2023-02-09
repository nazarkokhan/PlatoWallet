namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;

public record EverymatrixGetBalanceRequest(
    Guid Token,
    string Currency,
    string Hash) : IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>, IEveryMatrixRequest
{
    public class Handler : IRequestHandler<EverymatrixGetBalanceRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == request.Token))
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
                return EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.TokenNotFound);

            var response = new EverymatrixBalanceResponse(user.Balance, user.Currency);

            return EverymatrixResultFactory.Success(response);
        }
    }
}