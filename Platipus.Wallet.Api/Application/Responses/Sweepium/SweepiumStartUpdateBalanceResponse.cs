using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium;

public sealed record SweepiumStartUpdateBalanceResponse(
    [property: JsonPropertyName("currencyId")] string CurrencyId,
    [property: JsonPropertyName("totalBalance")] int TotalBalance,
    [property: JsonPropertyName("userID")] int UserId) : SweepiumCommonResponse;