namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi.Requests;

public sealed record EmaraplayAwardGameApiRequest(
    string User, 
    string Count, 
    string EndDate, 
    string Currency, 
    List<string>? Games = null, 
    string? Code = null,
    string? MinBet = "0", 
    string? MaxBet = null, 
    string? StartDate = null, 
    string? Operator = null);