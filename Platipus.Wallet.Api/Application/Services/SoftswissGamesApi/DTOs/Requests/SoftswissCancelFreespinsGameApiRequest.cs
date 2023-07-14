namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

using PswGamesApi.DTOs.Requests;

public record SoftswissCancelFreespinsGameApiRequest(string CasinoId, string IssueId) : IPswGamesApiBaseRequest;