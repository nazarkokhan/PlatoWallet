namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Base;

using System.Text.Json.Serialization;
using Newtonsoft.Json;

public sealed record UranusFailureResponse(
    [property: JsonProperty("error"), JsonPropertyOrder(2)] UranusCommonErrorResponse Error)
{
    [JsonProperty("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyOrder(1)]
    [JsonProperty("data")]
    public object? Data { get; init; } = null;
}