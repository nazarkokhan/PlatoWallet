namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CasinoGamesConfiguration : IEntityTypeConfiguration<CasinoGames>
{
    public void Configure(EntityTypeBuilder<CasinoGames> builder)
    {
        builder.ToTable("casino_games");
    }
}