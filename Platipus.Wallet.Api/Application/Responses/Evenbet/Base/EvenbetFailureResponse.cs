namespace Platipus.Wallet.Api.Application.Responses.Evenbet.Base;

using System.Text.Json.Serialization;

public sealed record EvenbetFailureResponse(
    [property: JsonPropertyName("error")] EvenbetErrorResponse Error);