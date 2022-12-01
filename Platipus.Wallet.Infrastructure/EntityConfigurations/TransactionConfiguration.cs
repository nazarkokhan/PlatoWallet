namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.Property(x => x.Amount).HasPrecision(38, 2);
        // builder.HasIndex(x => x.InternalId).IsUnique();
    }
}