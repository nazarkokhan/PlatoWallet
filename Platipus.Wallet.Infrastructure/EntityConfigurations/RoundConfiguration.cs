namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RoundConfiguration : EntityTypeConfiguration<Round, string>
{
    public override void Configure(EntityTypeBuilder<Round> builder)
    {
        base.Configure(builder);
        builder.ToTable("rounds");

        builder.Property(x => x.InternalId).HasDefaultValueSqlNewGuid().ValueGeneratedOnAdd();
    }
}