namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CasinoCurrenciesConfiguration : IEntityTypeConfiguration<CasinoCurrencies>
{
    public void Configure(EntityTypeBuilder<CasinoCurrencies> builder)
    {
        builder.ToTable("CasinoCurrencies");
    }
}