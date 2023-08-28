namespace Platipus.Wallet.Api.Application.Responses.Evenbet.Base;

using System.Text.Json.Serialization;

public abstract record EvenbetCommonResponse(
    [property: JsonPropertyName("balance")] int Balance,
    [property: JsonPropertyName("timestamp")] string Timestamp);