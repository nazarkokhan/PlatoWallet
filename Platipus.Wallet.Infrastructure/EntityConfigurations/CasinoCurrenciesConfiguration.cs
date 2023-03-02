namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CasinoCurrenciesConfiguration : EntityTypeConfiguration<CasinoCurrencies>
{
    public override void Configure(EntityTypeBuilder<CasinoCurrencies> builder)
    {
        base.Configure(builder);
        builder.ToTable("casino_currencies");
    }
}