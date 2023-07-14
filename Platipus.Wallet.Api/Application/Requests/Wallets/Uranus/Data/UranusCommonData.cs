namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Data;

using System.Text.Json.Serialization;

public abstract record UranusCommonData(
    [property: JsonPropertyName("currency")] string? Currency,
    [property: JsonPropertyName("balance")] decimal Balance);