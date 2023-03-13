namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;

using DTOs.Requests;
using DTOs.Responses;

public interface ISoftswissGamesApiClient
{
    Task<ISoftswissResult<SoftswissGetGameLinkGameApiResponse>> GetLaunchUrlAsync(
        Uri baseUrl,
        string casinoId,
        string user,
        string sessionId,
        int gameId,
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