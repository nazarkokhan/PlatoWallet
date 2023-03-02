namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : EntityTypeConfiguration<User, int>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);
        builder.ToTable("users");

        builder.HasIndex(x => x.Username).IsUnique();

        builder.Property(x => x.Balance).DefaultMoneyPrecision();
    }
}