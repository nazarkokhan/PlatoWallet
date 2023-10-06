namespace Platipus.Wallet.Api.Application.Requests.Admin.Currencies;

using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;

public sealed record RemoveCurrenciesFromCasinoRequest(
    [property: JsonPropertyName("casinoId")] string CasinoId,
    [property: JsonPropertyName("currencies")] List<string> Currencies) : IRequest<
    IResult<RemoveCurrenciesFromCasinoRequest.RemoveCurrenciesFromCasinoResponse>>
{
    public sealed class Handler : IRequestHandler<RemoveCurrenciesFromCasinoRequest,
        IResult<RemoveCurrenciesFromCasinoResponse>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => _walletDbContext = walletDbContext;

        public async Task<IResult<RemoveCurrenciesFromCasinoResponse>> Handle(
            RemoveCurrenciesFromCasinoRequest request,
            CancellationToken cancellationToken)
        {
            var casino = await _walletDbContext.Set<Casino>()
               .Include(c => c.CasinoCurrencies)
               .ThenInclude(c => c.Currency)
               .SingleOrDefaultAsync(c => c.Id == request.CasinoId, cancellationToken);

            if (casino is null)
            {
                return ResultFactory.Failure<RemoveCurrenciesFromCasinoResponse>(ErrorCode.CasinoNotFound);
            }

            var existingCurrencies = _walletDbContext.Set<Casino>()
               .Where(c => c.Id == request.CasinoId)
               .SelectMany(c => c.CasinoCurrencies)
               .ToList();

            var isExist = request.Currencies.Exists(currency => existingCurrencies.Exists(ec => ec.CurrencyId == currency));
            if (!isExist)
                return ResultFactory.Failure<RemoveCurrenciesFromCasinoResponse>(ErrorCode.InvalidCurrency);

            const string sqlToExecute = @"
                                DELETE FROM casino_currencies 
                                WHERE casino_id = {0} AND currency_id = {1};
                            ";

            foreach (var currencyCode in request.Currencies)
            {
                await _walletDbContext.Database.ExecuteSqlRawAsync(
                    sqlToExecute,
                    casino.Id,
                    currencyCode);
            }

            await _walletDbContext.SaveChangesAsync(cancellationToken);

            var casinoCurrencies = await _walletDbContext.Set<Casino>()
               .Where(c => c.Id == request.CasinoId)
               .SelectMany(c => c.CasinoCurrencies)
               .Select(x => x.CurrencyId)
               .ToListAsync(cancellationToken);

            var response = new RemoveCurrenciesFromCasinoResponse(casino.Id, casinoCurrencies);

            return ResultFactory.Success(response);
        }
    }

    public sealed record RemoveCurrenciesFromCasinoResponse(
        [property: JsonPropertyName("casinoId")] string CasinoId,
        [property: JsonPropertyName("currencies")] List<string> Currencies);
}