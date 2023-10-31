using System.Text.Json.Serialization;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Data;

public record SweepiumStartUpdateBalanceData([property: JsonPropertyName("token")] string Token);