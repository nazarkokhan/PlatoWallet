namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CasinoGamesConfiguration : EntityTypeConfiguration<CasinoGames>
{
    public override void Configure(EntityTypeBuilder<CasinoGames> builder)
    {
        base.Configure(builder);
        builder.ToTable("casino_games");
    }
}