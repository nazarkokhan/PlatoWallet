namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Data;

using System.Text.Json.Serialization;

public sealed record UranusCommonDataWithTransaction(
    string? Currency,
    decimal Balance,
    [property: JsonPropertyName("processedTransactionId")] string ProcessedTransactionId) : EvoplayCommonData(Currency, Balance);