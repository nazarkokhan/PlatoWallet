namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CasinoGameEnvironmentsConfiguration : EntityTypeConfiguration<CasinoGameEnvironments>
{
    public override void Configure(EntityTypeBuilder<CasinoGameEnvironments> builder)
    {
        base.Configure(builder);
        builder.ToTable("casino_game_environments");
    }
}