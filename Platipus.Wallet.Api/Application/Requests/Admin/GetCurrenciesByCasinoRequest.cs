namespace Platipus.Wallet.Api.Application.Requests.Admin;

using System.Text.Json.Serialization;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed record GetCurrenciesByCasinoRequest(
    [property: JsonPropertyName("casinoId")] string CasinoId) : IRequest<IResult<List<string>>>
{
    public sealed class Handler : IRequestHandler<GetCurrenciesByCasinoRequest, IResult<List<string>>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => _walletDbContext = walletDbContext;

        public async Task<IResult<List<string>>> Handle(
            GetCurrenciesByCasinoRequest request,
            CancellationToken cancellationToken)
        {
            var casino = await _walletDbContext.Set<Casino>()
               .Include(c => c.CasinoCurrencies)
               .SingleOrDefaultAsync(c => c.Id == request.CasinoId, cancellationToken);

            if (casino is null)
            {
                return ResultFactory.Failure<List<string>>(ErrorCode.CasinoNotFound);
            }

            var currenciesByCasino = await _walletDbContext.Set<Casino>()
               .Include(cc => cc.CasinoCurrencies)
               .Where(c => c.Id == request.CasinoId)
               .SelectMany(x => x.CasinoCurrencies)
               .Select(c => c.CurrencyId)
               .ToListAsync(cancellationToken);

            return ResultFactory.Success(currenciesByCasino);
        }
    }
}