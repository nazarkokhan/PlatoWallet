namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Data;

using System.Text.Json.Serialization;

public abstract record EvoplayCommonData(
    [property: JsonPropertyName("currency")] string? Currency,
    [property: JsonPropertyName("balance")] decimal Balance);