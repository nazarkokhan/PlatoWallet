namespace Platipus.Wallet.Api.Application.Requests.Admin.Currencies;

using System.Text.Json.Serialization;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed record RemoveCurrenciesRequest([property: JsonPropertyName("currencies")] HashSet<string> Currencies)
    : IRequest<IResult<HashSet<string>>>
{
    public sealed class Handler : IRequestHandler<RemoveCurrenciesRequest, IResult<HashSet<string>>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => _walletDbContext = walletDbContext;

        public async Task<IResult<HashSet<string>>> Handle(
            RemoveCurrenciesRequest request,
            CancellationToken cancellationToken)
        {
            var existingCurrencies = _walletDbContext.Set<Currency>().Select(x => x.Id).ToHashSet();

            var isExist = request.Currencies.Any(currency => existingCurrencies.Contains(currency));

            if (!isExist)
                return ResultFactory.Failure<HashSet<string>>(ErrorCode.InvalidCurrency);

            const string sqlToExecute = @"DELETE FROM currencies WHERE id = {0};";

            foreach (var currencyCode in request.Currencies)
            {
                await _walletDbContext.Database.ExecuteSqlRawAsync(
                    sqlToExecute,
                    currencyCode);
            }

            await _walletDbContext.SaveChangesAsync(cancellationToken);

            var actualCurrencies = _walletDbContext.Set<Currency>().Select(x => x.Id).ToHashSet();
            return ResultFactory.Success(actualCurrencies);
        }
    }
}