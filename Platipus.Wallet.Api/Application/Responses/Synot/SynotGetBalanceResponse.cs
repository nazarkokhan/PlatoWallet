namespace Platipus.Wallet.Api.Application.Responses.Synot;

using System.Text.Json.Serialization;

public sealed record SynotGetBalanceResponse([property: JsonPropertyName("balance")] long Balance);