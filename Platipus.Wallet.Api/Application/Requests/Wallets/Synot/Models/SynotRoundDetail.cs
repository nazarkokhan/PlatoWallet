namespace Platipus.Wallet.Api.Application.Requests.Wallets.Synot.Models;

using System.Text.Json.Serialization;

public sealed record SynotRoundDetail(
    [property: JsonPropertyName("totalBet")] long TotalBet,
    [property: JsonPropertyName("totalWin")] long TotalWin,
    [property: JsonPropertyName("baseWin")] long BaseWin,
    [property: JsonPropertyName("gambleCount")] int GambleCount,
    [property: JsonPropertyName("gambleWin")] long GambleWin,
    [property: JsonPropertyName("gambleBet")] long GambleBet,
    [property: JsonPropertyName("freeSpinCount")] int FreeSpinCount,
    [property: JsonPropertyName("freeSpinWin")] long FreeSpinWin,
    [property: JsonPropertyName("scratchCardTicketNumber")] string ScratchCardTicketNumber,
    [property: JsonPropertyName("scratchCardEmissionId")] string ScratchCardEmissionId);