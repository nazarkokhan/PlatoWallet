namespace Platipus.Wallet.Api.Extensions;

public static class OpenboxHelpers
{
    public static int? ToCurrencyCode(string currency)
    {
        return currency switch
        {
            "CNY" => 1,
            "USD" => 2,
            "MYR" => 3,
            "NVD" => 4,
            "THB" => 5,
            "IDR" => 6,
            "JPY" => 7,
            "KRW" => 8,
            "EUR" => 9,
            "AUD" => 10,
            "BND" => 11,
            "CAD" => 12,
            "CHF" => 13,
            "GBP" => 14,
            "MMK" => 15,
            "NOK" => 16,
            "NZD" => 17,
            "PHP" => 18,
            "SGD" => 19,
            "SEK" => 20,
            "ZAR" => 21,
            "ZWD" => 22,
            "HKD" => 23,
            "TWD" => 24,
            _ => null
        };
    }
}