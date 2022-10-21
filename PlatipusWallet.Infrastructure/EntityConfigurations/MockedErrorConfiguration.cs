namespace PlatipusWallet.Infrastructure.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MockedErrorConfiguration : IEntityTypeConfiguration<MockedError>
{
    public void Configure(EntityTypeBuilder<MockedError> builder)
    {
        builder.ToTable("MockedErrors");
    }
}