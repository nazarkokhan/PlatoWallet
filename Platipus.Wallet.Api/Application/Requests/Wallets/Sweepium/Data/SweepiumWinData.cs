using System.Text.Json.Serialization;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Data;

public record SweepiumWinData(string Token,
    string RoundId,
    string TransactionId,
    string GameId,
    [property: JsonPropertyName("currencyId")] string CurrencyId,
    [property: JsonPropertyName("winAmount")] string WinAmount) : SweepiumCommonData(Token, RoundId, TransactionId, GameId);