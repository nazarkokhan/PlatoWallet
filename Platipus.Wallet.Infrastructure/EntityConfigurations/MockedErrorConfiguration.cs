namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MockedErrorConfiguration : IEntityTypeConfiguration<MockedError>
{
    public void Configure(EntityTypeBuilder<MockedError> builder)
    {
        builder.ToTable("mocked_errors");
    }
}