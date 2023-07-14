namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Base;

using System.Text.Json.Serialization;
using Newtonsoft.Json;

public sealed record UranusSuccessResponse<TData>([property: JsonPropertyName("data"), JsonPropertyOrder(1)] TData Data)
{
    [JsonProperty("success")]
    public bool Success { get; init; } = true;

    [JsonPropertyOrder(2)]
    [JsonProperty("error")]
    public object? Error { get; init; } = null;
}