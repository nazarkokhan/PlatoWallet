namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("game"); //TODO rename to "games"

        builder.HasIndex(x => x.GameServerId).IsUnique();
        builder.HasIndex(x => x.LaunchName).IsUnique();
    }
}