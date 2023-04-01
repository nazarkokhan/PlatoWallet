namespace Platipus.Wallet.Api.Extensions;

using Application.Requests.Wallets.Openbox;

public static class OpenboxHelpers
{
    public const string VerifyPlayer = "e80355cb500e491f8ec067e54ba4e1e4",
                        GetPlayerInformation = "21a28abbc3744c47b03113e27b465475",
                        GetPlayerBalance = "96e7fc4ac82c4629b23da5296d25ec61",
                        MoneyTransactions = "b02cb70be50e41468aabf8d32237a3d4",
                        CancelTransaction = "68f28eb3c925488e95eb470670bc8827",
                        Logout = "562db5a013634b7195b1d0c650c414cf",
                        KeepTokenAlive = "09caee7b676f4c1c95050cd2e0bb5074";

    public static Type? GetRequestType(string method)
    {
        var payloadType = method switch
        {
            VerifyPlayer => typeof(OpenboxVerifyPlayerRequest),
            GetPlayerInformation => typeof(OpenboxGetPlayerInfoRequest),
            GetPlayerBalance => typeof(OpenboxBalanceRequest),
            MoneyTransactions => typeof(OpenboxMoneyTransactionRequest),
            CancelTransaction => typeof(OpenboxCancelTransactionRequest),
            Logout => typeof(OpenboxLogoutRequest),
            KeepTokenAlive => typeof(OpenboxKeepTokenAliveRequest),
            _ => null
        };

        return payloadType;
    }

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