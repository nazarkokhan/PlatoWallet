namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed record GetCurrenciesListRequest : IRequest<IResult<List<string>>>
{
    public sealed class Handler : IRequestHandler<GetCurrenciesListRequest, IResult<List<string>>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => _walletDbContext = walletDbContext;

        public async Task<IResult<List<string>>> Handle(
            GetCurrenciesListRequest request,
            CancellationToken cancellationToken)
        {
            var currencies = await _walletDbContext.Set<Currency>()
               .Select(x => x.Id)
               .ToListAsync(cancellationToken);

            return ResultFactory.Success(currencies);
        }
    }
}