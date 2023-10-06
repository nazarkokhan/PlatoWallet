namespace Platipus.Wallet.Api.Application.Requests.Admin.Currencies;

using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;

public sealed record AddCurrenciesRequest([property: JsonPropertyName("currencies")] HashSet<string> Currencies) : IRequest<
    IResult<HashSet<string>>>
{
    public sealed class Handler : IRequestHandler<AddCurrenciesRequest, IResult<HashSet<string>>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => _walletDbContext = walletDbContext;

        public async Task<IResult<HashSet<string>>> Handle(
            AddCurrenciesRequest request,
            CancellationToken cancellationToken)
        {
            var existingCurrencies = _walletDbContext.Set<Currency>().Select(x => x.Id).ToHashSet();

            var isExist = request.Currencies.Any(currency => existingCurrencies.Contains(currency));

            if (isExist)
                return ResultFactory.Failure<HashSet<string>>(ErrorCode.InvalidCurrency);

            const string sqlToExecute = @"INSERT INTO currencies (id, created_date)
                   SELECT {0}, {1}
                   WHERE NOT EXISTS (
                       SELECT 1
                       FROM currencies
                       WHERE id = {0}
                   );";

            foreach (var currencyCode in request.Currencies)
            {
                await _walletDbContext.Database.ExecuteSqlRawAsync(
                    sqlToExecute,
                    currencyCode,
                    DateTime.UtcNow);
            }

            await _walletDbContext.SaveChangesAsync(cancellationToken);

            var actualCurrencies = _walletDbContext.Set<Currency>().Select(x => x.Id).ToHashSet();
            return ResultFactory.Success(actualCurrencies);
        }
    }
}