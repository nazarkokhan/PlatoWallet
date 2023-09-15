namespace Platipus.Wallet.Api.Application.Responses.Vegangster;

using System.Text.Json.Serialization;
using Base;

public sealed record VegangsterTransactionResponse(
    string Status,
    string Currency,
    int Balance,
    [property: JsonPropertyName("external_transaction_id")] string ExternalTransactionId) : VegangsterCommonResponse(
    Status,
    Currency,
    Balance);