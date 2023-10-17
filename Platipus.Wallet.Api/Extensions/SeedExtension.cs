// ReSharper disable CollectionNeverUpdated.Global

#pragma warning disable CS8618
namespace Platipus.Wallet.Api.Extensions;

using System.Text.Json;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public static class SeedExtension
{
    // ReSharper disable once ClassNeverInstantiated.Local
    public record MigrationDefaultCasinoDto
    {
        public string Id { get; set; }
        public WalletProvider Provider { get; set; }
        public string SignatureKey { get; set; }
        public int? InternalId { get; set; }
        public string GameEnvironmentId { get; set; }
        public Casino.SpecificParams Params { get; set; }
        public List<string> CasinoCurrencies { get; set; }
        public List<string> CasinoGames { get; set; }
    }

    public static async Task<IApplicationBuilder> SeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

        dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

        await dbContext.Database.MigrateAsync();

        var dbContextTransaction = await dbContext.Database.BeginTransactionAsync();

        var anyCasino = dbContext.Set<Casino>().Any();
        if (!anyCasino)
        {
            var games = await dbContext.Set<Game>()
               .Select(
                    g => new
                    {
                        g.Id,
                        g.LaunchName
                    })
               .ToListAsync();

            var text = await File.ReadAllTextAsync("StaticFiles/default_casinos.json");
            var serialize = JsonSerializer.Deserialize<List<MigrationDefaultCasinoDto>>(text)!;

            var casinos = serialize
               .Select(
                    x =>
                    {
                        var casinoCurrencies = x.CasinoCurrencies
                           .Select(c => new CasinoCurrencies { CurrencyId = c })
                           .ToList();
                        var casinoGames = x.CasinoGames
                           .Select(c => new CasinoGames { GameId = games.Single(g => g.LaunchName == c).Id })
                           .ToList();
                        return new Casino(
                            x.Id,
                            x.Provider,
                            x.SignatureKey)
                        {
                            CasinoGameEnvironments = new List<CasinoGameEnvironments>()
                            {
                                new()
                                {
                                    GameEnvironmentId = x.GameEnvironmentId
                                }
                            },
                            Params = x.Params,
                            InternalId = x.InternalId ?? 0,
                            CasinoCurrencies = casinoCurrencies,
                            CasinoGames = casinoGames
                        };
                    })
               .ToList();

            dbContext.AddRange(casinos);
            await dbContext.SaveChangesAsync();
        }

        await dbContextTransaction.CommitAsync();

        return app;
    }
}