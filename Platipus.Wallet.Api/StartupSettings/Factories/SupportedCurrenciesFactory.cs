namespace Platipus.Wallet.Api.StartupSettings.Factories;

using Domain.Entities;
using Infrastructure.Persistence;
using Options;

public sealed class SupportedCurrenciesFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SupportedCurrenciesFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    public SupportedCurrenciesOptions Create()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
        var currencies = context.Set<Currency>()
           .Select(c => c.Id)
           .ToList();
        var options = new SupportedCurrenciesOptions
        {
            Items = new HashSet<string>(currencies)
        };
        return options;
    }
}