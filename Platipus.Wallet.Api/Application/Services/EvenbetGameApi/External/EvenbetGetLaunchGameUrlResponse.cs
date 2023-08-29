namespace Platipus.Wallet.Api.Application.Services.EvenbetGameApi.External;

using System.Text.Json.Serialization;

public sealed record EvenbetGetLaunchGameUrlResponse([property: JsonPropertyName("url")] Uri Url);