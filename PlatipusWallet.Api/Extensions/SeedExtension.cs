namespace PlatipusWallet.Api.Extensions;

using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public static class SeedExtension
{
    public static IApplicationBuilder Seed(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

        dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

        dbContext.Database.Migrate();

        return app;
    }
}