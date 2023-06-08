namespace Platipus.Wallet.Infrastructure.Persistence;

using EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);

        modelBuilder.HasDbFunction(() => NpgsqlFunctions.CurrentTimestamp())
            .IsBuiltIn()
            .HasTranslation(_ => new SqlFragmentExpression("current_timestamp"));
    }
}