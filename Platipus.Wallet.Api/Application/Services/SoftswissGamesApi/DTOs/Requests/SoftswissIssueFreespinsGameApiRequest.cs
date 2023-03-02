namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

using Hub88GamesApi.DTOs.Requests;

public record SoftswissIssueFreespinsGameApiRequest(
    string CasinoId,
    string Currency,
    string IssueId,
    string[] Games,
    int FreespinsQuantity,
    int BetLevel,
    DateTime ValidUntil,
    SoftswissGamesApiUser User) : IPswGamesApiBaseRequest;