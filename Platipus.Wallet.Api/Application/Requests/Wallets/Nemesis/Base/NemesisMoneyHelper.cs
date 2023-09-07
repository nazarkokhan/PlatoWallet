namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Base;

using System.Text.Json;
using System.Text.Json.Serialization;
using global::Serilog;
using JetBrains.Annotations;

public static class NemesisMoneyHelper
{
    private static readonly Dictionary<string, Currency> Convertors;

    static NemesisMoneyHelper()
    {
        var nemesisCurrenciesJson = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Application/Requests/Wallets/Nemesis/Base/nemesis_currency.json");

        Convertors = new Dictionary<string, Currency>();

        if (!File.Exists(nemesisCurrenciesJson))
        {
            Log.Fatal("Nemesis Could not load currencies file {NemesisCurrenciesFile}", nemesisCurrenciesJson);
            return;
        }

        var json = File.ReadAllText(nemesisCurrenciesJson);
        if (string.IsNullOrWhiteSpace(json))
        {
            Log.Fatal("Nemesis currencies file is empty {NemesisCurrenciesFile}", nemesisCurrenciesJson);
            return;
        }

        var currencies = JsonSerializer.Deserialize<List<Currency>>(
            json,
            new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });

        if (currencies is null)
        {
            Log.Fatal("Nemesis Could not deserialize currencies file {NemesisCurrenciesFile}", nemesisCurrenciesJson);
            return;
        }

        Convertors = currencies.ToDictionary(c => c.Iso, c => c);
    }

    public static decimal ToBalance(decimal amount, string currency)
    {
        decimal multiplier;
        decimal converter;
        if (Convertors.TryGetValue(currency, out var currencyItem))
        {
            multiplier = currencyItem.Multiplier;
            converter = currencyItem.Converter;
        }
        else
        {
            multiplier = 1;
            converter = 1;
        }

        return amount * converter / multiplier;
    }

    public static long FromBalance(decimal balance, string currency, out decimal multiplier)
    {
        decimal converter;
        if (Convertors.TryGetValue(currency, out var currencyItem))
        {
            multiplier = currencyItem.Multiplier;
            converter = currencyItem.Converter;
        }
        else
        {
            multiplier = 1;
            converter = 1;
        }

        return (long)(multiplier * balance / converter);
    }

    [UsedImplicitly]
    private sealed record Currency(
        [property: JsonPropertyName("iso")] string Iso,
        [property: JsonPropertyName("symbol")] string Symbol,
        [property: JsonPropertyName("multiplier")] decimal Multiplier,
        [property: JsonPropertyName("crypto")] bool Crypto,
        [property: JsonPropertyName("converter")] decimal Converter);
}