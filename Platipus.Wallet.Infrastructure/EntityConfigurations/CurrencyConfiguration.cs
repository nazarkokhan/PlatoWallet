namespace Platipus.Wallet.Infrastructure.EntityConfigurations;

using System.Text.Json;
using Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CurrencyConfiguration : EntityTypeConfiguration<Currency, string>
{
    public override void Configure(EntityTypeBuilder<Currency> builder)
    {
        base.Configure(builder);
        builder.ToTable("currencies");

        var currenciesText = File.ReadAllText("StaticFiles/default_currencies.json");
        var supportedCurrencies = JsonSerializer.Deserialize<SupportedCurrencies>(currenciesText)!;
        var currencies = supportedCurrencies.Crypto
            .Concat(supportedCurrencies.Fiat)
            .Select(c => new Currency(c))
            .ToList();
        builder.HasData(currencies);
    }

    private record SupportedCurrencies(HashSet<string> Fiat, HashSet<string> Crypto);
}