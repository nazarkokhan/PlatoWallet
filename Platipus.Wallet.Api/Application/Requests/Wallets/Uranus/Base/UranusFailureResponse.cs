namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Base;

using System.Text.Json.Serialization;

public sealed record UranusFailureResponse(
    [property: JsonPropertyName("error"), JsonPropertyOrder(2)] UranusCommonErrorResponse Error)
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyOrder(1)]
    [JsonPropertyName("data")]
    public object? Data { get; init; } = null;
}