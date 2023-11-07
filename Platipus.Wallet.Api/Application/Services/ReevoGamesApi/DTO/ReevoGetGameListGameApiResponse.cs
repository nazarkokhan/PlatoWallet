namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi.DTO;

using System.Text.Json.Serialization;

public record ReevoGetGameListGameApiResponse(
    int Error,
    List<ReevoGetGameListGameApiResponse.ResponseItem> Response)
{
    public record ResponseItem(
        string Id,
        string Name,
        string Type,
        string Subcategory,
        string Details,
        string New,
        string System,
        string Position,
        string Category,
        string Licence,
        string Plays,
        string Rtp,
        string? Wagering,
        [property: JsonPropertyName("gamename")] string GameName,
        string Report,
        string IdHash,
        string IdParent,
        string IdHashParent,
        [property: JsonPropertyName("freerounds_supported")] bool? FreeRoundsSupported,
        bool? HasJackpot,
        bool? Mobile,
        bool? PlayForFunSupported,
        string Image,
        string ImagePreview,
        string ImageFilled,
        string ImageBackground,
        string ImageBw);
}