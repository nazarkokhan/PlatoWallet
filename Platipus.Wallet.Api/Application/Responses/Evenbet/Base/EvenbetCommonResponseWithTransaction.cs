namespace Platipus.Wallet.Api.Application.Responses.Evenbet.Base;

using System.Text.Json.Serialization;

public abstract record EvenbetCommonResponseWithTransaction(
    int Balance,
    string Timestamp,
    [property: JsonPropertyName("transactionId")] string TransactionId) : EvenbetCommonResponse(Balance, Timestamp);