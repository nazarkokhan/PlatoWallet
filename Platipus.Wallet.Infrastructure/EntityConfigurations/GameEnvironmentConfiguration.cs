namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GameEnvironmentConfiguration : EntityTypeConfiguration<GameEnvironment, string>
{
    public override void Configure(EntityTypeBuilder<GameEnvironment> builder)
    {
        base.Configure(builder);
        builder.ToTable("game_environments");

        const string defaultUisUrl = "https://platipusgaming.cloud/qa/integration/vivo/test/index.html";
        builder.HasData(
            new GameEnvironment(
                GameEnvironment.Default,
                new Uri("https://test.platipusgaming.com/"),
                new Uri(defaultUisUrl)));

        builder.HasData(
            new GameEnvironment(
                GameEnvironment.Local,
                new Uri("http://localhost:5143/"),
                new Uri(defaultUisUrl)));
    }
}