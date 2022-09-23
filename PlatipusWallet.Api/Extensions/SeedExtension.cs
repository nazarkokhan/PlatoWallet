namespace PlatipusWallet.Api.Extensions;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Options;

public static class SeedExtension
{
    public static IApplicationBuilder Seed(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

        dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

        dbContext.Database.Migrate();

        var any = dbContext.Set<Currency>().Any();

        if (!any)
        {
            var supportedCurrencies = app.ApplicationServices
                .GetRequiredService<IOptions<SupportedCurrenciesOptions>>()
                .Value;

            var currencies = supportedCurrencies.Crypto
                .Concat(supportedCurrencies.Fiat)
                .Select(
                    c => new Currency
                    {
                        Name = c,
                    });
            
            dbContext.AddRange(currencies);

            dbContext.SaveChanges();
        }

        return app;
    }
}