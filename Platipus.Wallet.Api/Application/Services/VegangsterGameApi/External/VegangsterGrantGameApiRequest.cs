namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.External;

using System.Text.Json.Serialization;

public sealed record VegangsterGrantGameApiRequest(
    [property: JsonPropertyName("brand_id")] string BrandId,
    [property: JsonPropertyName("player_id")] string PlayerId,
    string Reference,
    [property: JsonPropertyName("game_code")] string GameCode,
    int Rounds,
    [property: JsonPropertyName("rounds_bet")] int RoundsBet,
    string Currency,
    [property: JsonPropertyName("end_date")] DateTime EndDate,
    [property: JsonPropertyName("start_date")] DateTime StartDate);