using System.Text.Json.Serialization;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Data;

public record SweepiumBetData(
    string Token,
    string RoundId,
    string TransactionId,
    string GameId,
    [property: JsonPropertyName("currencyId")] string CurrencyId,
    [property: JsonPropertyName("betAmount")] int BetAmount) : SweepiumCommonData(Token, RoundId, TransactionId, GameId);