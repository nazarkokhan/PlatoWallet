namespace Platipus.Wallet.Api.Application.Services.DafabetGameApi.Requests;

using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

[PublicAPI]
public sealed record DafabetGetLaunchUrlGameApiRequest(
    [property: JsonPropertyName("brand")] [property: BindProperty(Name = "brand")] string Brand,
    [property: JsonPropertyName("gameCode")] [property: BindProperty(Name = "gameCode")] string GameCode,
    [property: JsonPropertyName("playerId")] [property: BindProperty(Name = "playerId")] string PlayerId,
    [property: JsonPropertyName("playerToken")] [property: BindProperty(Name = "playerToken")] string PlayerToken,
    [property: JsonPropertyName("currency")] [property: BindProperty(Name = "currency")] string Currency,
    [property: JsonPropertyName("device")] [property: BindProperty(Name = "device")] string Device,
    [property: JsonPropertyName("language")] [property: BindProperty(Name = "language")] string Language,
    [property: JsonPropertyName("hash")] [property: BindProperty(Name = "hash")] string Hash,
    [property: JsonPropertyName("lobbyUrl")] [property: BindProperty(Name = "lobbyUrl")] string LobbyUrl);