namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using System.Text.Json;
using System.Text.Json.Serialization;
using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GameConfiguration : EntityTypeConfiguration<Game, int>
{
    public override void Configure(EntityTypeBuilder<Game> builder)
    {
        base.Configure(builder);
        builder.ToTable("games");

        builder.HasIndex(x => x.GameServiceId).IsUnique();
        builder.HasIndex(x => x.LaunchName).IsUnique();

        var gameListText = File.ReadAllText("StaticFiles/integration_gamelist_seed.json");
        var gameList = JsonSerializer.Deserialize<GameList[]>(gameListText)!.OrderBy(g => g.Id).ToArray();

        var games = new List<Game>();
        for (var i = 1; i < gameList.Length; i++)
        {
            var g = gameList[i];
            var game = new Game
            {
                Id = i,
                GameServiceId = g.Id,
                Name = g.Name,
                LaunchName = g.LaunchName,
                CategoryId = g.CategoryId,
            };
            games.Add(game);
        }

        builder.HasData(games);
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private record GameList(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("launch_name")] string LaunchName,
        [property: JsonPropertyName("category_id")] int CategoryId);
}