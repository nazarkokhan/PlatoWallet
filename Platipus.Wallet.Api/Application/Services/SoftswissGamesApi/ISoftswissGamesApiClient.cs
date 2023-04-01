namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;

using DTOs.Requests;
using DTOs.Responses;

public interface ISoftswissGamesApiClient
{
    Task<IResult<SoftswissBoxGamesApiResponse<SoftswissGetGameLinkGameApiResponse>>> GetLaunchUrlAsync(
        Uri baseUrl,
        string casinoId,
        string user,
        string sessionId,
        int gameId,
        string currency,
        long balance,
        CancellationToken cancellationToken = default);

    Task<IResult<SoftswissBoxGamesApiResponse<string>>> IssueFreespinsAsync(
        Uri baseUrl,
        SoftswissIssueFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<SoftswissBoxGamesApiResponse<SoftswissCancelFreespinsGameApiResponse>>> CancelFreespinsAsync(
        Uri baseUrl,
        SoftswissCancelFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<SoftswissBoxGamesApiResponse<SoftswissRoundDetailsGameApiResponse>>> RoundDetailsAsync(
        Uri baseUrl,
        SoftswissRoundDetailsGameApiRequest request,
        CancellationToken cancellationToken = default);
}