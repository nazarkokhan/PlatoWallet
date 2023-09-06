namespace Platipus.Wallet.Api.Application.Services.SynotGameApi.Requests;

using System.Text.Json.Serialization;

public sealed record SynotGetGameLaunchScriptGameApiRequest(
    string Game,
    string Token,
    string Currency,
    string Language,
    bool Real,
    [property: JsonPropertyName("lobbyUrl")] string LobbyUrl);