namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.SoftswissGamesApi;

using DTOs.Requests;
using DTOs.Responses;
using Results.HttpClient.WithData;

public interface ISoftswissGamesApiClient
{
    Task<IResult<IHttpClientResult<SoftswissGetGameLinkGameApiResponse, SoftswissGetGameLinkGameApiResponse>>> GetLaunchUrlAsync(
        Uri baseUrl,
        string casinoId,
        string user,
        int gameId,
        string currency,
        long balance,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<string, string>>> IssueFreespinsAsync(
        Uri baseUrl,
        SoftswissIssueFreespinsGameApiRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<SoftswissCancelFreespinsGameApiResponse, SoftswissCancelFreespinsGameApiResponse>>>
        CancelFreespinsAsync(
            Uri baseUrl,
            SoftswissCancelFreespinsGameApiRequest request,
            CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<SoftswissRoundDetailsGameApiResponse, SoftswissRoundDetailsGameApiResponse>>>
        RoundDetailsAsync(
            Uri baseUrl,
            SoftswissRoundDetailsGameApiRequest request,
            CancellationToken cancellationToken = default);
}