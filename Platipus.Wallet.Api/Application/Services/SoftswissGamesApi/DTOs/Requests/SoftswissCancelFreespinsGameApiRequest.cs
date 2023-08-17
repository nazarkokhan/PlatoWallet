namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

public record SoftswissCancelFreespinsGameApiRequest(string CasinoId, string IssueId) : ISoftswissGameApiBaseRequest;