namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AwardRoundConfiguration : EntityTypeConfiguration<AwardRound>
{
    public override void Configure(EntityTypeBuilder<AwardRound> builder)
    {
        base.Configure(builder);
        builder.ToTable("award_rounds");
    }
}