using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium;

public sealed record SweepiumStartUpdateBalanceResponse(
    bool Result,
    [property: JsonPropertyName("currencyId")] string CurrencyId,
    [property: JsonPropertyName("totalBalance")] int TotalBalance,
    [property: JsonPropertyName("userID")] int UserId) : SweepiumCommonResponse(
        Result);