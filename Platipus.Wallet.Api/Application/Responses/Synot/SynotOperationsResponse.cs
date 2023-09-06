namespace Platipus.Wallet.Api.Application.Responses.Synot;

using System.Text.Json.Serialization;

public sealed record SynotOperationsResponse(
    [property: JsonPropertyName("transactionId")] long TransactionId,
    long Balance);