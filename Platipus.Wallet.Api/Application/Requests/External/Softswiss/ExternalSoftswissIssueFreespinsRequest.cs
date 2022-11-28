namespace Platipus.Wallet.Api.Application.Requests.External.Softswiss;

using Extensions;
using Infrastructure.Persistence;
using Services.SoftswissGamesApi;
using Services.SoftswissGamesApi.DTOs.Requests;

public record ExternalSoftswissIssueFreespinsRequest(
        string CasinoId,
        string Currency,
        string IssueId,
        string[] Games,
        int FreespinsQuantity,
        int BetLevel,
        DateTime ValidUntil,
        string User)
    : IRequest<ISoftswissResult>
{
    public class Handler : IRequestHandler<ExternalSoftswissIssueFreespinsRequest, ISoftswissResult>
    {
        private readonly WalletDbContext _context;
        private readonly ISoftswissGamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, ISoftswissGamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<ISoftswissResult> Handle(
            ExternalSoftswissIssueFreespinsRequest request,
            CancellationToken cancellationToken)
        {
            var externalRequest = request.Map(
                r => new SoftswissIssueFreespinsGameApiRequest(
                    r.CasinoId,
                    r.Currency,
                    r.IssueId,
                    r.Games,
                    r.FreespinsQuantity,
                    r.BetLevel,
                    r.ValidUntil,
                    new SoftswissGamesApiUser(r.User, r.User)));

            var casinoGamesResponse = await _gamesApiClient.IssueFreespinsAsync(
                externalRequest,
                cancellationToken);

            return casinoGamesResponse;
        }
    }
}