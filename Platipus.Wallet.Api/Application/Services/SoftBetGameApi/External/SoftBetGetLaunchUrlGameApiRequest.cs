namespace Platipus.Wallet.Api.Application.Services.SoftBetGameApi.External;

using System.Text.Json.Serialization;

public sealed record SoftBetGetLaunchUrlGameApiRequest(
    [property: JsonPropertyName("providerGameId")] int ProviderGameId,
    [property: JsonPropertyName("licenseeId")] int LicenseeId,
    string Operator,
    string Token,
    string Username,
    string Currency,
    string Country,
    [property: JsonPropertyName("isbSkinId")] int IsbSkinId,
    [property: JsonPropertyName("isbGameId")] int IsbGameId,
    string Mode,
    [property: JsonPropertyName("launcherCode")] string LauncherCode,
    string Language,
    string Extra);