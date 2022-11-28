namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;

using DTOs.Requests;
using DTOs.Responses;

public interface ISoftswissGamesApiClient
{
    Task<ISoftswissResult<SoftswissGetGameLinkGameApiResponse>> GetLaunchUrlAsync(
        string casinoId,
        string user,
        Guid sessionId,
        string game,
        string currency,
        long balance,
        CancellationToken cancellationToken = default);

    Task<ISoftswissResult> IssueFreespinsAsync(
        SoftswissIssueFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<ISoftswissResult> CancelFreespinsAsync(
        SoftswissCancelFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<ISoftswissResult<SoftswissRoundDetailsGameApiResponse>> RoundDetailsAsync(
        SoftswissRoundDetailsGameApiRequest request,
        CancellationToken cancellationToken = default);
}