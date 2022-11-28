namespace Platipus.Wallet.Api.Application.Requests.External.Softswiss;

using Extensions;
using Services.SoftswissGamesApi;
using Services.SoftswissGamesApi.DTOs.Requests;

public record ExternalSoftswissCancelFreespinsRequest(
        string CasinoId,
        string IssueId)
    : IRequest<ISoftswissResult>
{
    public class Handler : IRequestHandler<ExternalSoftswissIssueFreespinsRequest, ISoftswissResult>
    {
        private readonly ISoftswissGamesApiClient _gamesApiClient;

        public Handler(ISoftswissGamesApiClient gamesApiClient)
        {
            _gamesApiClient = gamesApiClient;
        }

        public async Task<ISoftswissResult> Handle(
            ExternalSoftswissIssueFreespinsRequest request,
            CancellationToken cancellationToken)
        {
            var externalRequest = request.Map(
                r => new SoftswissCancelFreespinsGameApiRequest(
                    r.CasinoId,
                    r.IssueId));

            var casinoGamesResponse = await _gamesApiClient.CancelFreespinsAsync(
                externalRequest,
                cancellationToken);

            return casinoGamesResponse;
        }
    }
}