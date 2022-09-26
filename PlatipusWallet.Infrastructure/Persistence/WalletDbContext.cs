namespace PlatipusWallet.Infrastructure.Persistence;

using Domain.Entities;
using EntityConfigurations;
using Microsoft.EntityFrameworkCore;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}