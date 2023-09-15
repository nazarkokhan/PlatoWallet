namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.External;

using System.Text.Json.Serialization;

public sealed record VegangsterGetAvailableGamesGameApiRequest(
    [property: JsonPropertyName("operator_id")] string OperatorId,
    [property: JsonPropertyName("brand_id")] string BrandId);