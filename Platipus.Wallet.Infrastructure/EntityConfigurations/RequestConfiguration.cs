namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RequestConfiguration : EntityTypeConfiguration<Request, string>
{
    public override void Configure(EntityTypeBuilder<Request> builder)
    {
        base.Configure(builder);
        builder.ToTable("requests");

        builder.Property(x => x.Id).HasDefaultValueSqlNewGuid().ValueGeneratedOnAdd();
    }
}