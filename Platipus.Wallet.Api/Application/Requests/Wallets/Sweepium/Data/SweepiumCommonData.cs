using System.Text.Json.Serialization;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Data;

public abstract record SweepiumCommonData(
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("roundId")] string RoundId,
    [property: JsonPropertyName("transactionId")] string TransactionId,
    [property: JsonPropertyName("gameId")] string GameId);