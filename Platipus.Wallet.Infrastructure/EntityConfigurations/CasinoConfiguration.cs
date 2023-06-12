namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CasinoConfiguration : EntityTypeConfiguration<Casino, string>
{
    public override void Configure(EntityTypeBuilder<Casino> builder)
    {
        base.Configure(builder);
        builder.ToTable("casinos");

        //TODO
        builder.Property(x => x.InternalId).UseIdentityByDefaultColumn();
        builder.Property(x => x.Params).HasColumnType("jsonb");
        
        builder.Property(e => e.Provider)
            .HasConversion<int>();
    }
}