﻿namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.Requests;

using System.Text.Json.Serialization;

public sealed record UranusGetDemoLaunchUrlGameApiRequest(
    [property: JsonPropertyName("gameId")] string GameId,
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("platformType")] string PlatformType,
    [property: JsonPropertyName("depositUrl")] string DepositUrl,
    [property: JsonPropertyName("lobbyUrl")] string LobbyUrl,
    [property: JsonPropertyName("playerIp")] string PlayerIp);