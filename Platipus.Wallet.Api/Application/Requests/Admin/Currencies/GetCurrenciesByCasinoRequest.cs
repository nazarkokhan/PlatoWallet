namespace Platipus.Wallet.Api.Application.Requests.Admin.Currencies;

using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;

public sealed record GetCurrenciesByCasinoRequest(
    [property: JsonPropertyName("casinoId")] string CasinoId) : IRequest<IResult<HashSet<string>>>
{
    public sealed class Handler : IRequestHandler<GetCurrenciesByCasinoRequest, IResult<HashSet<string>>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => _walletDbContext = walletDbContext;

        public async Task<IResult<HashSet<string>>> Handle(
            GetCurrenciesByCasinoRequest request,
            CancellationToken cancellationToken)
        {
            var casino = await _walletDbContext.Set<Casino>()
               .Include(c => c.CasinoCurrencies)
               .SingleOrDefaultAsync(c => c.Id == request.CasinoId, cancellationToken);

            if (casino is null)
            {
                return ResultFactory.Failure<HashSet<string>>(ErrorCode.CasinoNotFound);
            }

            var currenciesByCasino = _walletDbContext.Set<Casino>()
               .Include(cc => cc.CasinoCurrencies)
               .Where(c => c.Id == request.CasinoId)
               .SelectMany(x => x.CasinoCurrencies)
               .Select(c => c.CurrencyId)
               .ToHashSet();

            return ResultFactory.Success(currenciesByCasino);
        }
    }
}