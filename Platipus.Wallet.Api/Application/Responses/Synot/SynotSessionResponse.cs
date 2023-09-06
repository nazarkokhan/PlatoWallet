namespace Platipus.Wallet.Api.Application.Responses.Synot;

using System.Text.Json.Serialization;

public sealed record SynotSessionResponse(
    [property: JsonPropertyName("playerId")] int PlayerId,
    [property: JsonPropertyName("gameId")] string GameId,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("token")] string Token);