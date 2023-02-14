namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo.Base;

using Microsoft.AspNetCore.Mvc;

public record ReevoSingleRequest2 : IReevoRequest
{
    [FromQuery(Name = "callerId")]
    public string CallerId { get; init; } = null!;

    [FromQuery(Name = "callerPassword")]
    public string CallerPassword { get; init; } = null!;

    [FromQuery(Name = "action")]
    public string Action { get; init; } = null!;

    // required for balance
    [FromQuery(Name = "remote_id")]
    public int? RemoteId { get; init; }

    [FromQuery(Name = "username")]
    public string Username { get; init; } = null!;

    [FromQuery(Name = "game_id_hash")]
    public string GameIdHash { get; init; } = null!;

    [FromQuery(Name = "session_id")]
    public string SessionId { get; init; } = null!;

    [FromQuery(Name = "gamesession_id")]
    public Guid GameSessionId { get; init; }

    [FromQuery(Name = "key")]
    public string Key { get; init; } = null!;

    //bet, win, cancel
    [FromQuery(Name = "amount")]
    public double? Amount { get; init; }

    //bet, win, cancel
    [FromQuery(Name = "transaction_id")]
    public string? TransactionId { get; init; }

    //bet, win, cancel
    [FromQuery(Name = "round_id")]
    public string? RoundId { get; init; }

    //bet, win, cancel
    [FromQuery(Name = "gameplay_final")]
    public int GameplayFinal { get; init; }

    //bet
    [FromQuery(Name = "is_freeround_bet")]
    public int? IsFreeRoundBet { get; init; }


    //bet, win
    [FromQuery(Name = "freeround_id")]
    public string? FreeRoundId { get; init; }

    //bet, win
    [FromQuery(Name = "fee")]
    public double? Fee { get; init; }

    //bet, win
    [FromQuery(Name = "jackpot_contribution_in_amount")]
    public double? JackpotContributionInAmount { get; init; }

    //win
    [FromQuery(Name = "is_freeround_win")]
    public int? IsFreeRoundWin { get; init; }

    //win
    [FromQuery(Name = "freeround_spins_remaining")]
    public int? FreeroundSpinsRemaining { get; init; }

    //win
    [FromQuery(Name = "freeround_completed")]
    public int? FreeroundCompleted { get; init; }

    //win
    [FromQuery(Name = "is_jackpot_win")]
    public int? IsJackpotWin { get; init; }

    //win
    [FromQuery(Name = "jackpot_win_in_amount")]
    public double? JackpotWinInAmount { get; init; }
}