namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

using Hub88GamesApi.DTOs.Requests;

public record SoftswissCancelFreespinsGameApiRequest(string CasinoId, string IssueId) : IPswGamesApiBaseRequest;