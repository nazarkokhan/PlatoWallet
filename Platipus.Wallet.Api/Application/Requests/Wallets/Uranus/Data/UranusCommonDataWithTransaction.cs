namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Data;

using System.Text.Json.Serialization;

public sealed record UranusCommonDataWithTransaction(
    string? Currency,
    decimal Balance,
    [property: JsonPropertyName("processedTransactionId")] string ProcessedTransactionId) : UranusCommonData(Currency, Balance);