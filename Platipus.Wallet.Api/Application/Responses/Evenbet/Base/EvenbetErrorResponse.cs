namespace Platipus.Wallet.Api.Application.Responses.Evenbet.Base;

using System.Text.Json.Serialization;

public sealed record EvenbetErrorResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("message")] string Message);