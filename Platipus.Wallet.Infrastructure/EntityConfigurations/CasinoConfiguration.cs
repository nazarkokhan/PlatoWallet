namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Platipus.Wallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CasinoConfiguration : IEntityTypeConfiguration<Casino>
{
    public void Configure(EntityTypeBuilder<Casino> builder)
    {
        builder.ToTable("Casinos");
        
    }
}