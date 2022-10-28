namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Platipus.Wallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AwardRoundConfiguration : IEntityTypeConfiguration<AwardRound>
{
    public void Configure(EntityTypeBuilder<AwardRound> builder)
    {
        builder.ToTable("AwardRounds");
    }
}