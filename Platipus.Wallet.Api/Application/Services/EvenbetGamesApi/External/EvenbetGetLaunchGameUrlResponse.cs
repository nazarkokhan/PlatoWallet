namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi.External;

using Newtonsoft.Json;

public sealed record EvenbetGetLaunchGameUrlResponse([property: JsonProperty("url")] Uri Url);