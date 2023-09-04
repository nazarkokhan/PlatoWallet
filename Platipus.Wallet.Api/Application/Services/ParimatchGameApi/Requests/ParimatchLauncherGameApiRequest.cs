namespace Platipus.Wallet.Api.Application.Services.ParimatchGameApi.Requests;

using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

[PublicAPI]
public record ParimatchLauncherGameApiRequest(
    [property: BindProperty(Name = "cid")] string Cid,
    [property: BindProperty(Name = "productid")] string ProductId,
    [property: BindProperty(Name = "sessiontoken")] string SessionToken,
    [property: BindProperty(Name = "lang")] string Lang,
    [property: BindProperty(Name = "lobbyurl")] string LobbyUrl,
    [property: BindProperty(Name = "targetchannel")] string TargetChannel,
    [property: BindProperty(Name = "providerid")] string ProviderId,
    [property: BindProperty(Name = "consumerid")] string ConsumerId);