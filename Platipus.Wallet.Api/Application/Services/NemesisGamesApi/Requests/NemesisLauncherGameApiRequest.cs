namespace Platipus.Wallet.Api.Application.Services.NemesisGamesApi.Requests;

using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

[PublicAPI]
public sealed record NemesisLauncherGameApiRequest(
    [property: BindProperty(Name = "session-token")] string SessionToken,
    [property: BindProperty(Name = "game-id")] string GameId,
    [property: BindProperty(Name = "lang")] string Lang,
    [property: BindProperty(Name = "user-id")] string UserId,
    [property: BindProperty(Name = "currency")] string Currency,
    [property: BindProperty(Name = "casino-id")] string CasinoId,
    [property: BindProperty(Name = "platform")] string? Platform,
    [property: BindProperty(Name = "lobby")] string Lobby);