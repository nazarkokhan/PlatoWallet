namespace PlatipusWallet.Api.Extensions;

using Bogus;
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
            var dbContextTransaction = dbContext.Database.BeginTransaction();

            var supportedCurrencies = app.ApplicationServices
                .GetRequiredService<IOptions<SupportedCurrenciesOptions>>()
                .Value;

            var currencies = supportedCurrencies.Crypto
                .Concat(supportedCurrencies.Fiat)
                .Select(
                    c => new Currency
                    {
                        Name = c,
                    })
                .ToList();

            dbContext.AddRange(currencies);

            dbContext.SaveChanges();

            app.SeedFakeData(dbContext);
            
            dbContextTransaction.Commit();
        }

        return app;
    }

    private static IApplicationBuilder SeedFakeData(this IApplicationBuilder app, DbContext dbContext)
    {
        var currencies = dbContext.Set<Currency>()
            .ToList();

        Randomizer.Seed = new Random(1234567890);

        var casinos = new Faker<Casino>()
            .RuleFor(x => x.Id, x => x.Random.Word().ToLower().Replace(' ', '_'))
            .RuleFor(x => x.SignatureKey, x => x.IndexGlobal.ToString())
            .Generate(20)
            .DistinctBy(x => x.Id)
            .ToList();

        casinos.ForEach(x => x.CasinoCurrencies = new Faker<CasinoCurrencies>()
            .RuleFor(c => c.CurrencyId, c => c.PickRandom(currencies).Id)
            .Generate(5)
            .DistinctBy(d => d.CurrencyId)
            .ToList());

        var users = new Faker<User>()
            .RuleFor(u => u.UserName, u => u.Person.UserName.ToLower())
            .RuleFor(u => u.Password, "qwe123")
            .RuleFor(u => u.Balance, u => u.Finance.Amount(-1000, 10000, 3))
            .RuleFor(u => u.CasinoId, u => u.PickRandom(casinos).Id)
            .Generate(1000)
            .DistinctBy(u => u.UserName)
            .ToList();
        
        users.ForEach(
            u => u.CurrencyId = new Faker()
                .PickRandom(
                    casinos
                        .First(x => x.Id == u.CasinoId)
                        .CasinoCurrencies)
                .CurrencyId);
        
        dbContext.AddRange(casinos);
        dbContext.AddRange(users);
        dbContext.SaveChanges();

        return app;
    }
}