namespace Platipus.Wallet.Api.Extensions;

using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StartupSettings.Options;

public static class SeedExtension
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private record GameList(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("launch_name")] string LaunchName,
        [property: JsonPropertyName("category_id")] int CategoryId);

    public static async Task<IApplicationBuilder> SeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

        dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

        await dbContext.Database.MigrateAsync();

        var dbContextTransaction = await dbContext.Database.BeginTransactionAsync();

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
                    })
                .ToList();

            dbContext.AddRange(currencies);

            await dbContext.SaveChangesAsync();

            await app.SeedFakeDataAsync(dbContext);
        }

        var anyGame = dbContext.Set<Game>().Any();

        if (!anyGame)
        {
            var gameListText = await File.ReadAllTextAsync("StaticFiles/integration_gamelist_seed.json");

            var gameList = JsonSerializer.Deserialize<GameList[]>(gameListText)!;

            var games = gameList.Select(
                    g => new Game
                    {
                        GameServerId = g.Id,
                        Name = g.Name,
                        LaunchName = g.LaunchName,
                        CategoryId = g.CategoryId,
                    })
                .ToList();

            dbContext.AddRange(games);

            await dbContext.SaveChangesAsync();
        }

        await dbContextTransaction.CommitAsync();

        return app;
    }

    private static async Task<IApplicationBuilder> SeedFakeDataAsync(this IApplicationBuilder app, DbContext dbContext)
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

        casinos.ForEach(
            x => x.CasinoCurrencies = new Faker<CasinoCurrencies>()
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
        await dbContext.SaveChangesAsync();

        return app;
    }
}