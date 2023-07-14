namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Base;

using System.Text.Json.Serialization;

public sealed record UranusFailureResponse(
    [property: JsonPropertyName("error"), JsonPropertyOrder(2)] EvoplayCommonErrorResponse Error)
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("data")]
    public object? Data { get; init; } = null;
}