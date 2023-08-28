namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi.External;

using System.Text.Json.Serialization;

public sealed record EvenbetGetLaunchGameUrlResponse([property: JsonPropertyName("url")] Uri Url);