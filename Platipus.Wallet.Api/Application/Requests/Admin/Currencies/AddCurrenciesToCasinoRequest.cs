namespace Platipus.Wallet.Api.Application.Requests.Admin.Currencies;

using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;

public sealed record AddCurrenciesToCasinoRequest(
    [property: JsonPropertyName("casinoId")] string CasinoId,
    [property: JsonPropertyName("currencies")] List<string> Currencies) : IRequest<
    IResult<AddCurrenciesToCasinoRequest.AddCurrenciesToCasinoResponse>>
{
    public sealed class Handler : IRequestHandler<AddCurrenciesToCasinoRequest, IResult<AddCurrenciesToCasinoResponse>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => _walletDbContext = walletDbContext;

        public async Task<IResult<AddCurrenciesToCasinoResponse>> Handle(
            AddCurrenciesToCasinoRequest request,
            CancellationToken cancellationToken)
        {
            var casino = await _walletDbContext.Set<Casino>()
               .Include(c => c.CasinoCurrencies)
               .ThenInclude(c => c.Currency)
               .SingleOrDefaultAsync(c => c.Id == request.CasinoId, cancellationToken);

            if (casino is null)
            {
                return ResultFactory.Failure<AddCurrenciesToCasinoResponse>(ErrorCode.CasinoNotFound);
            }

            var existingCurrencies = _walletDbContext.Set<Casino>()
               .Where(c => c.Id == request.CasinoId)
               .SelectMany(c => c.CasinoCurrencies)
               .ToList();

            var isExist = request.Currencies.Exists(currency => existingCurrencies.Exists(ec => ec.CurrencyId == currency));

            if (isExist)
                return ResultFactory.Failure<AddCurrenciesToCasinoResponse>(ErrorCode.InvalidCurrency);

            const string sqlToExecute = @"INSERT INTO casino_currencies (casino_id, currency_id, created_date)
                   SELECT {0}, {1}, {2}
                   WHERE NOT EXISTS (
                       SELECT 1
                       FROM casino_currencies
                       WHERE casino_id = {0} AND currency_id = {1}
                   );";

            foreach (var currencyCode in request.Currencies)
            {
                await _walletDbContext.Database.ExecuteSqlRawAsync(
                    sqlToExecute,
                    casino.Id,
                    currencyCode,
                    DateTime.UtcNow);
            }

            await _walletDbContext.SaveChangesAsync(cancellationToken);

            var casinoCurrencies = await _walletDbContext.Set<Casino>()
               .Where(c => c.Id == request.CasinoId)
               .SelectMany(c => c.CasinoCurrencies)
               .Select(x => x.CurrencyId)
               .ToListAsync(cancellationToken);

            var response = new AddCurrenciesToCasinoResponse(casino.Id, casinoCurrencies);

            return ResultFactory.Success(response);
        }
    }

    public sealed record AddCurrenciesToCasinoResponse(
        [property: JsonPropertyName("casinoId")] string CasinoId,
        [property: JsonPropertyName("currencies")] List<string> Currencies);
}