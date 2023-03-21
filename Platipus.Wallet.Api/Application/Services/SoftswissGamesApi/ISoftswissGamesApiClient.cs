namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;

using DTOs.Requests;
using DTOs.Responses;

public interface ISoftswissGamesApiClient
{
    Task<IResult<SoftswissGetGameLinkGameApiResponse>> GetLaunchUrlAsync(
        Uri baseUrl,
        string casinoId,
        string user,
        string sessionId,
        int gameId,
        string currency,
        long balance,
        CancellationToken cancellationToken = default);

    Task<IResult> IssueFreespinsAsync(
        Uri baseUrl,
        SoftswissIssueFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult> CancelFreespinsAsync(
        Uri baseUrl,
        SoftswissCancelFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<SoftswissRoundDetailsGameApiResponse>> RoundDetailsAsync(
        Uri baseUrl,
        SoftswissRoundDetailsGameApiRequest request,
        CancellationToken cancellationToken = default);
}