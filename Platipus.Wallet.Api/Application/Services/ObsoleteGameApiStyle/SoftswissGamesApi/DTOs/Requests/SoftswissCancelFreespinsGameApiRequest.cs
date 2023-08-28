namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.SoftswissGamesApi.DTOs.Requests;

public record SoftswissCancelFreespinsGameApiRequest(string CasinoId, string IssueId) : ISoftswissGameApiBaseRequest;