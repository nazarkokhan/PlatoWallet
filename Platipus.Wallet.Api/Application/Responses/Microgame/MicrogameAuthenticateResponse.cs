namespace Platipus.Wallet.Api.Application.Responses.Microgame;

using System.Text.Json.Serialization;
using Base;
using Results.Microgame;

public sealed record MicrogameAuthenticateResponse(
    [property: JsonPropertyName("externalId")] string ExternalId,
    [property: JsonPropertyName("userCurrency")] string UserCurrency,
    [property: JsonPropertyName("externalGameSessionId")] string ExternalGameSessionId,
    [property: JsonPropertyName("nickname")] string? Nickname = null,
    [property: JsonPropertyName("language")] string? Language = null,
    [property: JsonPropertyName("authSessionId")] string? AuthSessionId = null,
    [property: JsonPropertyName("authTicketId")] string? AuthTicketId = null,
    [property: JsonPropertyName("sessionType")] string? SessionType = null) : MicrogameCommonResponse(MicrogameStatusCode.OK);