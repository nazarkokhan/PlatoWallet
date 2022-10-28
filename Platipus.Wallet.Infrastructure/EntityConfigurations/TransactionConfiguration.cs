namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Platipus.Wallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        
        builder.Property(x => x.Amount).HasPrecision(38, 2);
    }
}