namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TransactionConfiguration : EntityTypeConfiguration<Transaction, string>
{
    public override void Configure(EntityTypeBuilder<Transaction> builder)
    {
        base.Configure(builder);
        builder.ToTable("transactions");

        builder.Property(x => x.Amount).DefaultMoneyPrecision();
        builder.Property(x => x.InternalId).HasDefaultValueSqlNewGuid().ValueGeneratedOnAdd();
    }
}