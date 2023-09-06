namespace Platipus.Wallet.Api.Application.Responses.Synot.Base;

using System.Text.Json.Serialization;

public sealed record SynotErrorResponse(
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("errorCode")] string Error);