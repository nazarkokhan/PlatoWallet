namespace Platipus.Wallet.Api.Application.Services.MicrogameGameApi.Responses;

using System.Text.Json.Serialization;

public sealed record MicrogameLaunchGameApiResponse(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("error")] int Error = 0);