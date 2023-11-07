using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium;

public record SweepiumSuccessResponse(
    [property: JsonPropertyName("transactionId")] string TransactionId,
    [property: JsonPropertyName("balance")] int Balance) : SweepiumCommonResponse;
    
