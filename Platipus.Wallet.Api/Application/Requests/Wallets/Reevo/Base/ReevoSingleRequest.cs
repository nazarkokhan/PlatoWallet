namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo.Base;

using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public record ReevoSingleRequest(
    [property: FromQuery(Name = "callerId")] string CallerId,
    [property: FromQuery(Name = "callerPassword")] string CallerPassword,
    [property: FromQuery(Name = "action")] string Action,
    [property: FromQuery(Name = "remote_id")] int? RemoteId,
    [property: FromQuery(Name = "username")] string Username,
    [property: FromQuery(Name = "game_id_hash")] string? GameIdHash,
    [property: FromQuery(Name = "session_id")] string SessionId,
    [property: FromQuery(Name = "gamesession_id")] string GameSessionId,
    [property: FromQuery(Name = "key")] string Key,
    [property: FromQuery(Name = "amount")] double? Amount,
    [property: FromQuery(Name = "transaction_id")] string? TransactionId,
    [property: FromQuery(Name = "round_id")] string? RoundId,
    [property: FromQuery(Name = "gameplay_final")] int GameplayFinal,
    [property: FromQuery(Name = "is_freeround_bet")] int? IsFreeRoundBet,
    [property: FromQuery(Name = "freeround_id")] string? FreeRoundId,
    [property: FromQuery(Name = "fee")] double? Fee,
    [property: FromQuery(Name = "jackpot_contribution_in_amount")] double? JackpotContributionInAmount,
    [property: FromQuery(Name = "is_freeround_win")] int? IsFreeRoundWin,
    [property: FromQuery(Name = "freeround_spins_remaining")] int? FreeroundSpinsRemaining,
    [property: FromQuery(Name = "freeround_completed")] int? FreeroundCompleted,
    [property: FromQuery(Name = "is_jackpot_win")] int? IsJackpotWin,
    [property: FromQuery(Name = "jackpot_win_in_amount")] double? JackpotWinInAmount) : IReevoRequest;