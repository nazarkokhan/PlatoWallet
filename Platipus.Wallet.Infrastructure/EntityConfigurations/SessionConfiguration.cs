namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SessionConfiguration : EntityTypeConfiguration<Session, string>
{
    public override void Configure(EntityTypeBuilder<Session> builder)
    {
        base.Configure(builder);
        builder.ToTable("sessions");

        builder.Property(x => x.Id).HasDefaultValueSqlNewGuid().ValueGeneratedOnAdd();
    }
}