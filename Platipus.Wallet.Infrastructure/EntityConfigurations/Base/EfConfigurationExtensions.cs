namespace Platipus.Wallet.Infrastructure.EntityConfigurations.Base;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class EfConfigurationExtensions
{
    private const string SqlUtcNow = "now()";
    private const string SqlNewGuid = "gen_random_uuid()";

    public static PropertyBuilder<DateTime?> HasDefaultValueSqlDateTimeUtcNow(this PropertyBuilder<DateTime?> propertyBuilder)
    {
        // return propertyBuilder.HasDefaultValue(NpgsqlFunctions.CurrentTimestamp());
        return propertyBuilder.HasDefaultValueSql(SqlUtcNow);
    }

    public static PropertyBuilder<DateTime> HasDefaultValueSqlDateTimeUtcNow(this PropertyBuilder<DateTime> propertyBuilder)
    {
        return propertyBuilder.HasDefaultValueSql(SqlUtcNow);
    }

    public static PropertyBuilder<string> HasDefaultValueSqlNewGuid(this PropertyBuilder<string> propertyBuilder)
    {
        return propertyBuilder.HasDefaultValueSql(SqlNewGuid);
    }

    public static PropertyBuilder<Guid> HasDefaultValueSqlNewGuid(this PropertyBuilder<Guid> propertyBuilder)
    {
        return propertyBuilder.HasDefaultValueSql(SqlNewGuid);
    }

    public static PropertyBuilder<Uri> HasConversionUri(this PropertyBuilder<Uri> propertyBuilder)
    {
        return propertyBuilder.HasConversion(v => v.ToString(), v => new Uri(v));
    }

    public static PropertyBuilder<decimal> DefaultMoneyPrecision(this PropertyBuilder<decimal> propertyBuilder)
    {
        return propertyBuilder.HasPrecision(28, 2);
    }
}