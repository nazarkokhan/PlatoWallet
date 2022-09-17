namespace PlatipusWallet.Infrastructure.Persistence;

using Domain.Entities;
using EntityConfigurations;
using Microsoft.EntityFrameworkCore;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    
    public DbSet<Game> UserGroups { get; set; }
    
    public DbSet<Round> Groups { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}