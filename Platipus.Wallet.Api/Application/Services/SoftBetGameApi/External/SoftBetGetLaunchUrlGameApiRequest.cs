namespace Platipus.Wallet.Api.Application.Services.SoftBetGameApi.External;

using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

[PublicAPI]
public sealed record SoftBetGetLaunchUrlGameApiRequest(
    [property: JsonPropertyName("providerGameId")] [property: BindProperty(Name = "providergameid")] int ProviderGameId,
    [property: JsonPropertyName("licenseeId")] [property: BindProperty(Name = "licenseeid")] int LicenseeId,
    [property: JsonPropertyName("playerId")] [property: BindProperty(Name = "playerid")] string PlayerId,
    [property: BindProperty(Name = "operator")] string Operator,
    [property: BindProperty(Name = "token")] string Token,
    [property: BindProperty(Name = "username")] string Username,
    [property: BindProperty(Name = "currency")] string Currency,
    [property: BindProperty(Name = "country")] string Country,
    [property: JsonPropertyName("isbSkinId")] [property: BindProperty(Name = "isbskinid")] int IsbSkinId,
    [property: JsonPropertyName("isbGameId")] [property: BindProperty(Name = "isbgameid")] int IsbGameId,
    [property: BindProperty(Name = "mode")] string Mode,
    [property: JsonPropertyName("launcherCode")] [property: BindProperty(Name = "launchercode")] string LauncherCode,
    [property: BindProperty(Name = "language")] string Language,
    [property: BindProperty(Name = "extra")] string Extra);