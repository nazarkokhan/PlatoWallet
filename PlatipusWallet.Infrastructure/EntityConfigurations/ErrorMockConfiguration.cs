namespace PlatipusWallet.Infrastructure.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ErrorMockConfiguration : IEntityTypeConfiguration<ErrorMock>
{
    public void Configure(EntityTypeBuilder<ErrorMock> builder)
    {
        builder.ToTable("ErrorMocks");
    }
}