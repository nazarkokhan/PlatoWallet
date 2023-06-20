namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi.Requests;

public sealed record EmaraplayGetRoundDetailsGameApiRequest(
    string Bet, 
    string? User = null, 
    string? Game = null,
    string? Operator = null, 
    string? Currency = null);