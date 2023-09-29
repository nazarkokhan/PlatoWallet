namespace Platipus.Wallet.Api.Application.Services.SoftBetGameApi.External;

using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

[PublicAPI]
public sealed record SoftBetGetLaunchUrlGameApiRequest(
    [property: BindProperty(Name = "providergameid")] int ProviderGameId,
    [property: BindProperty(Name = "licenseeid")] int LicenseeId,
    [property: BindProperty(Name = "playerid")] string PlayerId,
    [property: BindProperty(Name = "operator")] string Operator,
    [property: BindProperty(Name = "token")] string Token,
    [property: BindProperty(Name = "username")] string Username,
    [property: BindProperty(Name = "currency")] string Currency,
    [property: BindProperty(Name = "country")] string Country,
    [property: BindProperty(Name = "isbskinid")] int IsbSkinId,
    [property: BindProperty(Name = "isbgameid")] int IsbGameId,
    [property: BindProperty(Name = "mode")] string Mode,
    [property: BindProperty(Name = "launchercode")] string LauncherCode,
    [property: BindProperty(Name = "language")] string Language,
    [property: BindProperty(Name = "extra")] string Extra);