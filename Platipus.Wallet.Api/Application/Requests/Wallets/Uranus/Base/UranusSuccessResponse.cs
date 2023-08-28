﻿namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Base;

using System.Text.Json.Serialization;

public sealed record UranusSuccessResponse<TData>([property: JsonPropertyName("data"), JsonPropertyOrder(1)] TData Data)
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = true;

    [JsonPropertyOrder(2)]
    [JsonPropertyName("error")]
    public object? Error { get; init; } = null;
}