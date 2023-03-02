namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MockedErrorConfiguration : EntityTypeConfiguration<MockedError, int>
{
    public override void Configure(EntityTypeBuilder<MockedError> builder)
    {
        base.Configure(builder);
        builder.ToTable("mocked_errors");

        builder.HasIndex(
                x => new
                {
                    x.Method,
                    x.UserId,
                    x.ExecutionOrder
                })
            .IsUnique();
    }
}