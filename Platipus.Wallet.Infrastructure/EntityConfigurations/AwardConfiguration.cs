namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AwardConfiguration : EntityTypeConfiguration<Award, string>
{
    public override void Configure(EntityTypeBuilder<Award> builder)
    {
        base.Configure(builder);
        builder.ToTable("awards");
    }
}