namespace Platipus.Wallet.Api.Application.Services.SwGameApi.Responses;

using System.Text.Json.Serialization;

public sealed record SwGetGameBetsGameApiResponse(
    [property: JsonPropertyName("successCode")] string SuccessCode,
    [property: JsonPropertyName("gamesBets")] object GamesBets);