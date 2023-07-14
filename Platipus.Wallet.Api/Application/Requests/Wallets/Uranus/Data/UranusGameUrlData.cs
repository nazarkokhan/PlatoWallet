namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Data;

using System.Text.Json.Serialization;

public sealed record UranusGameUrlData([property: JsonPropertyName("url")] Uri Url);